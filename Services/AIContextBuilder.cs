using System.Text.RegularExpressions;
using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Models.KnowledgeArchitecture;

namespace MamiaSeedsOil.Web.Services;

public sealed class AIContextBuilder : IAIContextBuilder
{
    private static readonly Regex MultiSpaceRegex = new("\\s+", RegexOptions.Compiled);
    private readonly IKnowledgeService _knowledgeService;

    public AIContextBuilder(IKnowledgeService knowledgeService)
    {
        _knowledgeService = knowledgeService;
    }

    public async Task<AiQueryContext> BuildAsync(string question, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_knowledgeService.IsDomainQuestion(question))
        {
            return new AiQueryContext
            {
                Question = question,
                IsDomainQuestion = false,
                UnknownReason = UnknownQuestionReason.OutOfDomain
            };
        }

        var categories = await _knowledgeService.GetCategoriesAsync(cancellationToken);
        var entries = categories.SelectMany(category => category.Entries).ToArray();
        var normalizedQuestion = Normalize(question);

        var best = entries
            .Select(entry => new
            {
                Entry = entry,
                Score = Score(normalizedQuestion, entry)
            })
            .OrderByDescending(x => x.Score)
            .FirstOrDefault();

        if (best is null || best.Score <= 0)
        {
            return new AiQueryContext
            {
                Question = question,
                IsDomainQuestion = true,
                Categories = categories,
                UnknownReason = UnknownQuestionReason.NoMatchingEntry
            };
        }

        return new AiQueryContext
        {
            Question = question,
            IsDomainQuestion = true,
            Categories = categories,
            BestMatch = best.Entry,
            BestScore = best.Score,
            UnknownReason = best.Entry.IsUnavailable ? UnknownQuestionReason.MatchedUnavailableEntry : UnknownQuestionReason.None
        };
    }

    private static double Score(string normalizedQuestion, KnowledgeEntry entry)
    {
        var keywords = entry.Keywords
            .Where(keyword => !string.IsNullOrWhiteSpace(keyword))
            .Select(Normalize)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (keywords.Length == 0)
        {
            return 0;
        }

        var hits = keywords.Count(keyword => normalizedQuestion.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        if (hits == 0)
        {
            return 0;
        }

        var titleBoost = normalizedQuestion.Contains(Normalize(entry.Title), StringComparison.OrdinalIgnoreCase) ? 0.15 : 0;
        return (double)hits / keywords.Length + titleBoost;
    }

    private static string Normalize(string value)
    {
        var lowered = (value ?? string.Empty).Trim().ToLowerInvariant();
        return MultiSpaceRegex.Replace(lowered, " ");
    }
}
