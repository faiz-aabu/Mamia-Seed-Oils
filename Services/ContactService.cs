using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using MamiaSeedsOil.Web.Helpers;
using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Models;
using MamiaSeedsOil.Web.Resources;
using Microsoft.Extensions.Localization;

namespace MamiaSeedsOil.Web.Services;

public sealed class ContactService : IContactService
{
    private readonly IEnquiryStore _enquiryStore;
    private readonly IEmailNotificationService _emailNotificationService;
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly ILogger<ContactService> _logger;

    public ContactService(
        IEnquiryStore enquiryStore,
        IEmailNotificationService emailNotificationService,
        IStringLocalizer<SharedResource> localizer,
        ILogger<ContactService> logger)
    {
        _enquiryStore = enquiryStore;
        _emailNotificationService = emailNotificationService;
        _localizer = localizer;
        _logger = logger;
    }

    public async Task<ContactServiceResult> SubmitContactEnquiryAsync(ContactEnquiry enquiry, CancellationToken cancellationToken = default)
    {
        var validationResult = Validate(enquiry);
        if (!validationResult.Success)
        {
            return validationResult;
        }

        SanitizeEnquiry(enquiry);
        enquiry.SubmittedAtUtc = DateTimeOffset.UtcNow;
        await _enquiryStore.StoreContactEnquiryAsync(enquiry, cancellationToken);

        try
        {
            await _emailNotificationService.SendContactEnquiryNotificationAsync(enquiry, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email delivery failed for contact enquiry {EnquiryId}", enquiry.Id);
            return new ContactServiceResult
            {
                Success = false,
                Message = _localizer["EmailDeliveryFailed"],
                EnquiryId = enquiry.Id
            };
        }

        _logger.LogInformation(
            "Contact enquiry submitted. EnquiryId={EnquiryId}; Email={EmailMasked}; Company={Company}",
            enquiry.Id,
            PrivacyMasker.MaskEmail(enquiry.Email),
            enquiry.CompanyName);

        return new ContactServiceResult
        {
            Success = true,
            Message = _localizer["ContactSuccessMessage"],
            EnquiryId = enquiry.Id
        };
    }

    public async Task<ContactServiceResult> SubmitDistributorEnquiryAsync(DistributorEnquiry enquiry, CancellationToken cancellationToken = default)
    {
        var validationResult = Validate(enquiry);
        if (!validationResult.Success)
        {
            return validationResult;
        }

        SanitizeDistributorEnquiry(enquiry);
        enquiry.SubmittedAtUtc = DateTimeOffset.UtcNow;
        await _enquiryStore.StoreDistributorEnquiryAsync(enquiry, cancellationToken);

        try
        {
            await _emailNotificationService.SendDistributorEnquiryNotificationAsync(enquiry, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email delivery failed for distributor enquiry {EnquiryId}", enquiry.Id);
            return new ContactServiceResult
            {
                Success = false,
                Message = _localizer["EmailDeliveryFailed"],
                EnquiryId = enquiry.Id
            };
        }

        _logger.LogInformation(
            "Distributor enquiry submitted. EnquiryId={EnquiryId}; Email={EmailMasked}; BusinessType={BusinessType}",
            enquiry.Id,
            PrivacyMasker.MaskEmail(enquiry.Email),
            enquiry.BusinessType);

        return new ContactServiceResult
        {
            Success = true,
            Message = _localizer["ContactDistributorSuccessMessage"],
            EnquiryId = enquiry.Id
        };
    }

    private ContactServiceResult Validate<T>(T model)
    {
        var context = new ValidationContext(model!);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(model!, context, results, validateAllProperties: true);

        if (isValid)
        {
            return new ContactServiceResult { Success = true };
        }

        return new ContactServiceResult
        {
            Success = false,
            Message = results.FirstOrDefault()?.ErrorMessage ?? _localizer["ValidationFailed"]
        };
    }

    private static void SanitizeEnquiry(ContactEnquiry enquiry)
    {
        enquiry.FullName = SanitizeText(enquiry.FullName);
        enquiry.CompanyName = SanitizeText(enquiry.CompanyName);
        enquiry.Email = SanitizeText(enquiry.Email).Trim().ToLowerInvariant();
        enquiry.Phone = SanitizeText(enquiry.Phone);
        enquiry.Message = SanitizeText(enquiry.Message);
    }

    private static void SanitizeDistributorEnquiry(DistributorEnquiry enquiry)
    {
        enquiry.CompanyName = SanitizeText(enquiry.CompanyName);
        enquiry.ContactPerson = SanitizeText(enquiry.ContactPerson);
        enquiry.Email = SanitizeText(enquiry.Email).Trim().ToLowerInvariant();
        enquiry.Phone = SanitizeText(enquiry.Phone);
        enquiry.Country = SanitizeText(enquiry.Country);
        enquiry.State = SanitizeText(enquiry.State);
        enquiry.BusinessType = SanitizeText(enquiry.BusinessType);
        enquiry.ExpectedMonthlyVolume = SanitizeText(enquiry.ExpectedMonthlyVolume);
        enquiry.Message = SanitizeText(enquiry.Message);
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
