using System.ComponentModel.DataAnnotations;

namespace MamiaSeedsOil.Web.Configuration;

public sealed class CompanyProfileOptions
{
    public const string SectionName = "CompanyProfile";

    [Required]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    public DateOnly EstablishedDate { get; set; }

    [Required]
    public List<string> AddressLines { get; set; } = [];

    [Required]
    public string BusinessDescription { get; set; } = string.Empty;

    [Required]
    public string AdditionalProductsDescription { get; set; } = string.Empty;
}
