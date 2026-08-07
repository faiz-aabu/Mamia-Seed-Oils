using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using MamiaSeedsOil.Web.DTOs;
using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Models;

namespace MamiaSeedsOil.Web.Services;

public sealed class DistributorApplicationService : IDistributorApplicationService
{
    private readonly IEnquiryStore _enquiryStore;
    private readonly IEmailNotificationService _emailNotificationService;
    private readonly ILogger<DistributorApplicationService> _logger;
    private readonly ConcurrentDictionary<string, DateTimeOffset> _recentSubmissions = new(StringComparer.OrdinalIgnoreCase);

    public DistributorApplicationService(
        IEnquiryStore enquiryStore,
        IEmailNotificationService emailNotificationService,
        ILogger<DistributorApplicationService> logger)
    {
        _enquiryStore = enquiryStore;
        _emailNotificationService = emailNotificationService;
        _logger = logger;
    }

    public async Task<DistributorApplicationResponseDto> SubmitApplicationAsync(DistributorApplicationRequestDto request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!string.IsNullOrWhiteSpace(request.SpamTrap))
        {
            _logger.LogWarning("Distributor application blocked by spam trap field.");
            return new DistributorApplicationResponseDto { Success = false, Message = "Your submission could not be processed. Please try again." };
        }

        if (request.FormStartedAtUtc.HasValue)
        {
            var elapsed = DateTimeOffset.UtcNow - request.FormStartedAtUtc.Value;
            if (elapsed.TotalSeconds < 2)
            {
                _logger.LogWarning("Distributor application blocked by timing rule.");
                return new DistributorApplicationResponseDto { Success = false, Message = "Your submission could not be processed. Please try again." };
            }
        }

        _logger.LogInformation("✔ Distributor form received for {BusinessName}", request.BusinessName);

        var normalizedKey = BuildSubmissionKey(request);
        if (_recentSubmissions.TryGetValue(normalizedKey, out var lastSubmitted) && DateTimeOffset.UtcNow - lastSubmitted < TimeSpan.FromHours(24))
        {
            _logger.LogWarning("Duplicate distributor application blocked. Key={Key}", normalizedKey);
            return new DistributorApplicationResponseDto { Success = false, Message = "We already received a submission from this business recently. Please contact us directly for assistance." };
        }

        var application = MapToDomain(request);
        var validation = Validate(application);
        if (!validation.Success)
        {
            _logger.LogWarning("✘ Distributor application validation failed. Message={Message}", validation.Message);
            return validation;
        }

        _logger.LogInformation("✔ Model validation passed");
        Sanitize(application);
        await _enquiryStore.StoreDistributorApplicationAsync(application, cancellationToken);

        _logger.LogInformation("✔ Email service started");
        try
        {
            await _emailNotificationService.SendDistributorApplicationNotificationAsync(application, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email delivery failed for distributor application {ApplicationId}; the application was still stored and the user was redirected to success.", application.Id);
        }

        _recentSubmissions[normalizedKey] = DateTimeOffset.UtcNow;
        _logger.LogInformation("✔ Application accepted for follow-up. ApplicationId={ApplicationId}; BusinessName={BusinessName}", application.Id, application.BusinessName);

        return new DistributorApplicationResponseDto
        {
            Success = true,
            Message = "Thank you. Your distributor application has been submitted successfully.",
            ApplicationId = application.Id
        };
    }

    private DistributorApplicationResponseDto Validate(DistributorApplication application)
    {
        var context = new ValidationContext(application);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(application, context, results, validateAllProperties: true);

        if (isValid)
        {
            return new DistributorApplicationResponseDto { Success = true };
        }

        return new DistributorApplicationResponseDto
        {
            Success = false,
            Message = results.FirstOrDefault()?.ErrorMessage ?? "Validation failed."
        };
    }

    private static DistributorApplication MapToDomain(DistributorApplicationRequestDto request)
    {
        return new DistributorApplication
        {
            FullName = request.FullName,
            BusinessName = request.BusinessName,
            EmailAddress = request.EmailAddress,
            PhoneNumber = request.PhoneNumber,
            WhatsAppNumber = request.WhatsAppNumber,
            State = request.State,
            City = request.City,
            BusinessAddress = request.BusinessAddress,
            Country = request.Country,
            BusinessType = request.BusinessType,
            VehicleType = string.Equals(request.BusinessType, "Distributor", StringComparison.OrdinalIgnoreCase) ? request.VehicleType : null,
            NumberOfVehicles = string.Equals(request.BusinessType, "Distributor", StringComparison.OrdinalIgnoreCase) ? request.NumberOfVehicles : null,
            ExpectedMonthlyOrderQuantity = request.ExpectedMonthlyOrderQuantity,
            PreferredProducts = request.PreferredProducts?.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToList() ?? [],
            WarehouseAvailable = request.WarehouseAvailable,
            CanHandleBulkOrders = request.CanHandleBulkOrders,
            AreasYouCanSupply = request.AreasYouCanSupply ?? string.Empty,
            AgreedToTerms = request.AgreedToTerms,
            SpamTrap = request.SpamTrap,
            FormStartedAtUtc = request.FormStartedAtUtc,
            SubmittedAtUtc = DateTimeOffset.UtcNow
        };
    }

    private static string BuildSubmissionKey(DistributorApplicationRequestDto request)
    {
        var businessName = SanitizeText(request.BusinessName);
        var email = SanitizeText(request.EmailAddress).Trim().ToLowerInvariant();
        return $"{businessName}|{email}";
    }

    private static void Sanitize(DistributorApplication application)
    {
        application.FullName = SanitizeText(application.FullName);
        application.BusinessName = SanitizeText(application.BusinessName);
        application.EmailAddress = SanitizeText(application.EmailAddress).Trim().ToLowerInvariant();
        application.PhoneNumber = SanitizeText(application.PhoneNumber);
        application.WhatsAppNumber = SanitizeText(application.WhatsAppNumber);
        application.State = SanitizeText(application.State);
        application.City = SanitizeText(application.City);
        application.BusinessAddress = SanitizeText(application.BusinessAddress);
        application.Country = SanitizeText(application.Country);
        application.BusinessType = SanitizeText(application.BusinessType);
        application.VehicleType = SanitizeText(application.VehicleType);
        application.NumberOfVehicles = SanitizeText(application.NumberOfVehicles);
        application.ExpectedMonthlyOrderQuantity = SanitizeText(application.ExpectedMonthlyOrderQuantity);
        application.AreasYouCanSupply = SanitizeText(application.AreasYouCanSupply);
    }

    private static string SanitizeText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var sanitized = Regex.Replace(value, "<[^>]+>", string.Empty);
        sanitized = Regex.Replace(sanitized, @"\s+", " ");
        return sanitized.Trim();
    }
}
