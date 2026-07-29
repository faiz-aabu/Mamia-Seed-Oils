namespace MamiaSeedsOil.Web.Configuration;

public sealed class FeatureFlagsOptions
{
    public const string SectionName = "FeatureFlags";

    public bool EnableAiAssistant { get; set; } = true;
    public bool EnablePartnershipCentre { get; set; } = true;
    public bool EnableStatusEndpoints { get; set; } = true;
}
