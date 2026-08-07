using System.ComponentModel.DataAnnotations;
using MamiaSeedsOil.Web.Validation;

namespace MamiaSeedsOil.Web.DTOs;

public sealed class DistributorApplicationRequestDto : IValidatableObject
{
    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(160, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(180, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string BusinessName { get; set; } = string.Empty;

    [Required(ErrorMessage = "ValidationRequired")]
    [EmailAddress(ErrorMessage = "ValidationEmail")]
    [StringLength(180, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string EmailAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "ValidationRequired")]
    [Phone(ErrorMessage = "ValidationPhone")]
    [StringLength(60, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "ValidationRequired")]
    [Phone(ErrorMessage = "ValidationPhone")]
    [StringLength(60, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string WhatsAppNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(120, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string State { get; set; } = string.Empty;

    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(120, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(300, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string BusinessAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(120, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string Country { get; set; } = "Nigeria";

    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(80, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string BusinessType { get; set; } = string.Empty;

    [StringLength(80, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string? VehicleType { get; set; }

    [StringLength(40, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string? NumberOfVehicles { get; set; }

    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(120, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string ExpectedMonthlyOrderQuantity { get; set; } = string.Empty;

    public List<string>? PreferredProducts { get; set; }

    public bool? WarehouseAvailable { get; set; }

    public bool? CanHandleBulkOrders { get; set; }

    [StringLength(2000, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string? AreasYouCanSupply { get; set; }

    [Range(typeof(bool), "true", "true", ErrorMessage = "ValidationTermsRequired")]
    public bool AgreedToTerms { get; set; }

    [StringLength(120)]
    public string? SpamTrap { get; set; }

    public DateTimeOffset? FormStartedAtUtc { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.Equals(BusinessType, "Distributor", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(VehicleType))
            {
                yield return new ValidationResult("Vehicle Type is required when Business Type is Distributor.", [nameof(VehicleType)]);
            }
        }
    }
}

public sealed class DistributorApplicationResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? ApplicationId { get; set; }
}
