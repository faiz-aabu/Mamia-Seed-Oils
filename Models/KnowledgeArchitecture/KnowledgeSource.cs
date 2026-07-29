namespace MamiaSeedsOil.Web.Models.KnowledgeArchitecture;

public enum KnowledgeSourceType
{
    Json = 0,
    Pdf = 1,
    Docx = 2,
    Txt = 3,
    Markdown = 4,
    Excel = 5,
    Other = 6
}

public sealed class KnowledgeSource
{
    public KnowledgeSourceType SourceType { get; init; } = KnowledgeSourceType.Json;
    public string SourcePath { get; init; } = string.Empty;
    public string SourceId { get; init; } = string.Empty;
}
