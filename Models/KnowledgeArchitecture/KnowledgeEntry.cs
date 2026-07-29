namespace MamiaSeedsOil.Web.Models.KnowledgeArchitecture;

public sealed class KnowledgeEntry
{
    public string Id { get; init; } = string.Empty;
    public string CategoryKey { get; init; } = string.Empty;
    public string CategoryTitle { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public IReadOnlyList<string> Keywords { get; init; } = [];
    public KnowledgeSource Source { get; init; } = new();

    public bool IsUnavailable
    {
        get
        {
            var content = Content?.Trim();
            return string.IsNullOrWhiteSpace(content)
                || string.Equals(content, "[To Be Updated]", StringComparison.OrdinalIgnoreCase);
        }
    }
}
