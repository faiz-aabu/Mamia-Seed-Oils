namespace MamiaSeedsOil.Web.Configuration;

public sealed class ObservabilityOptions
{
    public const string SectionName = "Observability";

    public int SlowRequestThresholdMs { get; set; } = 1200;
    public bool EnableRequestStartLogs { get; set; }
}
