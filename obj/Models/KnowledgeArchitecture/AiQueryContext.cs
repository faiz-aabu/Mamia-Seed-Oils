namespace MamiaSeedsOil.Web.Models.KnowledgeArchitecture;

public enum UnknownQuestionReason
{
    None = 0,
    OutOfDomain = 1,
    NoMatchingEntry = 2,
    MatchedUnavailableEntry = 3
}

public sealed class AiQueryContext
{
    public string Question { get; init; } = string.Empty;
    public bool IsDomainQuestion { get; init; }
    public KnowledgeEntry? BestMatch { get; init; }
    public double BestScore { get; init; }
    public IReadOnlyList<KnowledgeCategory> Categories { get; init; } = [];
    public UnknownQuestionReason UnknownReason { get; init; }
}
