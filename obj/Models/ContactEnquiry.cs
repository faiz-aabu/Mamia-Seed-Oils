using System.ComponentModel.DataAnnotations;
using MamiaSeedsOil.Web.Validation;

namespace MamiaSeedsOil.Web.Models;

public sealed class ContactEnquiry
{
    public Guid Id { get; set; } = Guid.NewGuid();

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

    public DateTimeOffset SubmittedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
