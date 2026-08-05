namespace MamiaSeedsOil.Web.Configuration;

public sealed class FutureDataConnectionsOptions
{
    public const string SectionName = "FutureDataConnections";

    public string PrimaryConnectionString { get; set; } = string.Empty;
    public string ReadReplicaConnectionString { get; set; } = string.Empty;
    public string Provider { get; set; } = "[To Be Updated]";
}
