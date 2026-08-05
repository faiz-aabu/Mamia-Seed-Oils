using System.ComponentModel.DataAnnotations;
using MamiaSeedsOil.Web.DTOs.Partnership;
using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Models;
using MamiaSeedsOil.Web.Resources;
using Microsoft.Extensions.Localization;

namespace MamiaSeedsOil.Web.Services;

public sealed class PartnershipApplicationService : IPartnershipApplicationService
{
    private readonly IEnquiryStore _enquiryStore;
    private readonly IEmailNotificationService _emailNotificationService;
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly ILogger<PartnershipApplicationService> _logger;

    public PartnershipApplicationService(
        IEnquiryStore enquiryStore,
        IEmailNotificationService emailNotificationService,
        IStringLocalizer<SharedResource> localizer,
        ILogger<PartnershipApplicationService> logger)
    {
        _enquiryStore = enquiryStore;
        _emailNotificationService = emailNotificationService;
        _localizer = localizer;
        _logger = logger;
    }

    public async Task<PartnershipApplicationResponseDto> SubmitApplicationAsync(PartnershipApplicationRequestDto request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!string.IsNullOrWhiteSpace(request.SpamTrap))
        {
            _logger.LogWarning("Partnership application blocked by spam trap field.");
            return new PartnershipApplicationResponseDto
            {
                Success = false,
                Message = _localizer["PartnershipSpamBlocked"]
            };
        }

        if (request.FormStartedAtUtc.HasValue)
        {
            var elapsed = DateTimeOffset.UtcNow - request.FormStartedAtUtc.Value;
            if (elapsed.TotalSeconds < 2)
            {
                _logger.LogWarning("Partnership application blocked by timing rule.");
                return new PartnershipApplicationResponseDto
                {
                    Success = false,
                    Message = _localizer["PartnershipSpamBlocked"]
                };
            }
        }

        var application = MapToDomain(request);
        var validation = Validate(application);

        if (!validation.Success)
        {
            return validation;
        }

        await _enquiryStore.StorePartnershipApplicationAsync(application, cancellationToken);
        await _emailNotificationService.SendPartnershipApplicationNotificationAsync(application, cancellationToken);

        _logger.LogInformation("Partnership application submitted. ApplicationId={ApplicationId}; Company={Company}", application.Id, application.CompanyName);

        return new PartnershipApplicationResponseDto
        {
            Success = true,
            Message = _localizer["PartnershipSubmitSuccess"],
            ApplicationId = application.Id,
            Status = application.Status
        };
    }

    private PartnershipApplicationResponseDto Validate(PartnershipApplication application)
    {
        var context = new ValidationContext(application);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(application, context, validationResults, validateAllProperties: true);

        if (isValid)
        {
            return new PartnershipApplicationResponseDto { Success = true };
        }

        return new PartnershipApplicationResponseDto
        {
            Success = false,
            Message = validationResults.FirstOrDefault()?.ErrorMessage ?? _localizer["ValidationFailed"]
        };
    }

    private static PartnershipApplication MapToDomain(PartnershipApplicationRequestDto request)
    {
        return new PartnershipApplication
        {
            CompanyName = request.CompanyName,
            BusinessRegistrationNumber = string.IsNullOrWhiteSpace(request.BusinessRegistrationNumber) ? "[To Be Updated]" : request.BusinessRegistrationNumber,
            ContactPerson = request.ContactPerson,
            Position = string.IsNullOrWhiteSpace(request.Position) ? "[To Be Updated]" : request.Position,
            PhoneNumber = request.PhoneNumber,
            EmailAddress = request.EmailAddress,
            BusinessAddress = string.IsNullOrWhiteSpace(request.BusinessAddress) ? "[To Be Updated]" : request.BusinessAddress,
            Country = request.Country,
            State = request.State,
            City = request.City,
            BusinessType = request.BusinessType,
            YearsInOperation = string.IsNullOrWhiteSpace(request.YearsInOperation) ? "[To Be Updated]" : request.YearsInOperation,
            MonthlyPurchaseEstimate = request.MonthlyPurchaseEstimate,
            PreferredProducts = request.PreferredProducts?.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToList() ?? [],
            PreferredPackaging = request.PreferredPackaging?.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToList() ?? [],
            AdditionalNotes = request.AdditionalNotes ?? string.Empty,
            AgreedToTerms = request.AgreedToTerms,
            Status = PartnershipApplicationStatus.Submitted,
            SubmittedAtUtc = DateTimeOffset.UtcNow,
            Documents =
            [
                new PartnershipDocumentMetadata { DocumentType = "Business Registration Certificate" },
                new PartnershipDocumentMetadata { DocumentType = "Company Profile" },
                new PartnershipDocumentMetadata { DocumentType = "Tax Documents" },
                new PartnershipDocumentMetadata { DocumentType = "Supporting Documents" }
            ]
        };
    }
}
