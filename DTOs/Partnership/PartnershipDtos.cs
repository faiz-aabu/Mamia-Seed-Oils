using System.ComponentModel.DataAnnotations;
using MamiaSeedsOil.Web.Models;
using MamiaSeedsOil.Web.Validation;

namespace MamiaSeedsOil.Web.DTOs.Partnership;

public sealed class PartnershipApplicationRequestDto
{
    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(180, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string CompanyName { get; set; } = string.Empty;

    [StringLength(120, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string? BusinessRegistrationNumber { get; set; }

    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(120, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string ContactPerson { get; set; } = string.Empty;

    [StringLength(120, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string? Position { get; set; }

    [Required(ErrorMessage = "ValidationRequired")]
    [Phone(ErrorMessage = "ValidationPhone")]
    [StringLength(60, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "ValidationRequired")]
    [EmailAddress(ErrorMessage = "ValidationEmail")]
    [StringLength(180, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string EmailAddress { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string? BusinessAddress { get; set; }

    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(80, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string Country { get; set; } = string.Empty;

    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(80, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string State { get; set; } = string.Empty;

    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(80, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(120, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string BusinessType { get; set; } = string.Empty;

    [StringLength(80, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string? YearsInOperation { get; set; }

    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(120, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string MonthlyPurchaseEstimate { get; set; } = string.Empty;

    public List<string>? PreferredProducts { get; set; }
    public List<string>? PreferredPackaging { get; set; }

    [StringLength(2000, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string? AdditionalNotes { get; set; }

    [Range(typeof(bool), "true", "true", ErrorMessage = "ValidationTermsRequired")]
    public bool AgreedToTerms { get; set; }

    [StringLength(120)]
    public string? SpamTrap { get; set; }

    public DateTimeOffset? FormStartedAtUtc { get; set; }
}

public sealed class PartnershipApplicationResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? ApplicationId { get; set; }
    public PartnershipApplicationStatus Status { get; set; } = PartnershipApplicationStatus.Submitted;
}
