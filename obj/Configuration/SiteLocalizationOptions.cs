using System.ComponentModel.DataAnnotations;

namespace MamiaSeedsOil.Web.Configuration;

public sealed class SiteLocalizationOptions
{
    public const string SectionName = "Localization";

    [Required]
    public string DefaultCulture { get; set; } = "en";

    [Required]
    public List<string> SupportedCultures { get; set; } = ["en", "ha"];
}
