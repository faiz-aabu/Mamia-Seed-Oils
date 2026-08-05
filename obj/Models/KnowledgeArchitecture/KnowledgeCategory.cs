namespace MamiaSeedsOil.Web.Models.KnowledgeArchitecture;

public sealed class KnowledgeCategory
{
    public string Key { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IReadOnlyList<string> Keywords { get; init; } = [];
    public IReadOnlyList<KnowledgeEntry> Entries { get; init; } = [];
}
