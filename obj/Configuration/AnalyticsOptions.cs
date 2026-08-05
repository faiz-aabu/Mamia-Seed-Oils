namespace MamiaSeedsOil.Web.Configuration;

public sealed class AnalyticsOptions
{
    public const string SectionName = "Analytics";

    public string GoogleAnalytics4Id { get; set; } = string.Empty;
    public string GoogleTagManagerId { get; set; } = string.Empty;
    public string MicrosoftClarityId { get; set; } = string.Empty;
    public string MetaPixelId { get; set; } = string.Empty;
}
