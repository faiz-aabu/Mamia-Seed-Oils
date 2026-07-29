using System.ComponentModel.DataAnnotations;
using MamiaSeedsOil.Web.Validation;

namespace MamiaSeedsOil.Web.Models;

public enum PartnershipApplicationStatus
{
    Submitted = 0,
    UnderReview = 1,
    Contacted = 2,
    Approved = 3,
    Rejected = 4,
    Archived = 5
}

public sealed class PartnershipApplication
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(180, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string CompanyName { get; set; } = string.Empty;

    [StringLength(120, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string BusinessRegistrationNumber { get; set; } = "[To Be Updated]";

    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(120, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string ContactPerson { get; set; } = string.Empty;

    [StringLength(120, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string Position { get; set; } = "[To Be Updated]";

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
    public string BusinessAddress { get; set; } = "[To Be Updated]";

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
    public string YearsInOperation { get; set; } = "[To Be Updated]";

    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(120, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string MonthlyPurchaseEstimate { get; set; } = string.Empty;

    public List<string> PreferredProducts { get; set; } = [];
    public List<string> PreferredPackaging { get; set; } = [];

    [StringLength(2000, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string AdditionalNotes { get; set; } = string.Empty;

    [Range(typeof(bool), "true", "true", ErrorMessage = "ValidationTermsRequired")]
    public bool AgreedToTerms { get; set; }

    public PartnershipApplicationStatus Status { get; set; } = PartnershipApplicationStatus.Submitted;
    public DateTimeOffset SubmittedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public List<PartnershipDocumentMetadata> Documents { get; set; } = [];
    public List<PartnershipNote> Notes { get; set; } = [];
    public List<PartnershipCommunicationHistory> CommunicationHistory { get; set; } = [];
}

public sealed class PartnershipDocumentMetadata
{
    public string DocumentType { get; set; } = "[To Be Updated]";
    public string OriginalFileName { get; set; } = "[To Be Updated]";
    public string StorageReference { get; set; } = "[To Be Updated]";
    public string ProcessingStatus { get; set; } = "[To Be Updated]";
    public DateTimeOffset UploadedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class PartnershipNote
{
    public string Author { get; set; } = "System";
    public string Note { get; set; } = "[To Be Updated]";
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class PartnershipCommunicationHistory
{
    public string Channel { get; set; } = "[To Be Updated]";
    public string Summary { get; set; } = "[To Be Updated]";
    public string Direction { get; set; } = "[To Be Updated]";
    public DateTimeOffset OccurredAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
