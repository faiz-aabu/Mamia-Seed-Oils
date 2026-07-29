using System.ComponentModel.DataAnnotations;
using MamiaSeedsOil.Web.Validation;

namespace MamiaSeedsOil.Web.Models;

public sealed class DistributorEnquiry
{
    public Guid Id { get; set; } = Guid.NewGuid();

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

    public DateTimeOffset SubmittedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
