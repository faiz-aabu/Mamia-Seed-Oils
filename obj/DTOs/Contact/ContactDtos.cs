using System.ComponentModel.DataAnnotations;
using MamiaSeedsOil.Web.Validation;

namespace MamiaSeedsOil.Web.DTOs.Contact;

public sealed class ContactEnquiryRequestDto
{
    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(120, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(120, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string? CompanyName { get; set; }

    [Required(ErrorMessage = "ValidationRequired")]
    [EmailAddress(ErrorMessage = "ValidationEmail")]
    [StringLength(180, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "ValidationPhone")]
    [StringLength(60, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string? Phone { get; set; }

    [StringLength(2000, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string? Message { get; set; }
}

public sealed class DistributorEnquiryRequestDto
{
    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(160, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(120, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string ContactPerson { get; set; } = string.Empty;

    [Required(ErrorMessage = "ValidationRequired")]
    [EmailAddress(ErrorMessage = "ValidationEmail")]
    [StringLength(180, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "ValidationRequired")]
    [Phone(ErrorMessage = "ValidationPhone")]
    [StringLength(60, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(80, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string Country { get; set; } = string.Empty;

    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(80, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string State { get; set; } = string.Empty;

    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(120, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string BusinessType { get; set; } = string.Empty;

    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(120, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string ExpectedMonthlyVolume { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string? Message { get; set; }
}

public sealed class ContactResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? EnquiryId { get; set; }
}
