namespace MamiaSeedsOil.Web.Configuration;

public sealed class ImageDeliveryOptions
{
    public const string SectionName = "ImageDelivery";

    public bool UseCdn { get; set; }
    public string CdnBaseUrl { get; set; } = string.Empty;
    public string DefaultSizes { get; set; } = "(max-width: 900px) 100vw, 33vw";
    public string BlurPlaceholderDataUrl { get; set; } = string.Empty;
}
