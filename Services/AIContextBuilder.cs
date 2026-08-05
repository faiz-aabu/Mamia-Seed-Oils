using System.Text.RegularExpressions;
using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Models.AiAssistant;
using MamiaSeedsOil.Web.Models.KnowledgeArchitecture;

namespace MamiaSeedsOil.Web.Services;

public sealed class AIContextBuilder : IAIContextBuilder
{
    private static readonly Regex MultiSpaceRegex = new("\\s+", RegexOptions.Compiled);
    private static readonly Regex PunctuationRegex = new("[^a-z0-9\\s]", RegexOptions.Compiled);
    private static readonly HashSet<string> FollowUpAnchors =
    [
        "it", "that", "this", "there", "them", "those", "these", "visit", "cost", "price", "how", "when", "where", "can", "do"
    ];

    private static readonly Dictionary<string, string[]> IntentSynonyms = new(StringComparer.OrdinalIgnoreCase)
    {
        ["factory-location"] = ["factory", "location", "address", "where are you", "kaduna", "makarfi", "airport road", "visit"],
        ["products"] = ["products", "what do you sell", "what do you produce", "catalog", "cooking oil", "soyabean meal", "soybean meal", "by-products"],
        ["bulk-supply"] = ["bulk", "wholesale", "industrial", "cartons", "supermarket", "hotel", "restaurant", "government contract"],
        ["distribution"] = ["distributor", "distribution", "delivery", "nationwide", "supply network", "retail"],
        ["certifications"] = ["nafdac", "son", "certification", "quality", "compliance"],
        ["contact"] = ["contact", "phone", "email", "call", "whatsapp", "customer support", "complaint"],
        ["careers"] = ["career", "job", "employment", "vacancy", "internship"],
        ["partnerships"] = ["partnership", "partner", "business collaboration", "become a distributor"]
    };

    private static readonly HashSet<string> DomainAnchorWords =
    [
        "mamia", "seed", "seeds", "oil", "soya", "soyabean", "soybean", "meal", "factory", "kaduna", "makarfi", "nafdac", "son",
        "distribution", "distributor", "wholesale", "retail", "bulk", "packaging", "carton", "delivery", "product", "products",
        "partnership", "support", "contact", "email", "phone", "visit", "industrial", "livestock", "poultry", "fish", "export"
    ];

    private readonly IKnowledgeService _knowledgeService;

    public AIContextBuilder(IKnowledgeService knowledgeService)
    {
        _knowledgeService = knowledgeService;
    }

    public async Task<AiQueryContext> BuildAsync(
        string question,
        IReadOnlyList<AiConversationMessage>? conversationHistory = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var normalizedQuestion = Normalize(question);
        var expandedQuestion = ExpandWithConversation(question, conversationHistory);
        var normalizedExpandedQuestion = Normalize(expandedQuestion);
        var questionTokens = Tokenize(normalizedExpandedQuestion);

        var isDomainQuestion = _knowledgeService.IsDomainQuestion(question)
            || ContainsDomainAnchors(questionTokens)
            || (conversationHistory?.Count > 0 && LooksLikeFollowUp(normalizedQuestion));

        if (!isDomainQuestion)
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

        var best = entries
            .Select(entry => new
            {
                Entry = entry,
                Score = Score(normalizedExpandedQuestion, questionTokens, entry)
            })
            .OrderByDescending(x => x.Score)
            .FirstOrDefault();

        if (best is null || best.Score < 0.22)
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

    private static bool ContainsDomainAnchors(IReadOnlyList<string> tokens)
    {
        return tokens.Any(token => DomainAnchorWords.Contains(token));
    }

    private static bool LooksLikeFollowUp(string normalizedQuestion)
    {
        var tokens = Tokenize(normalizedQuestion);
        if (tokens.Count == 0)
        {
            return false;
        }

        return tokens.Count <= 7 && tokens.Any(token => FollowUpAnchors.Contains(token));
    }

    private static string ExpandWithConversation(string question, IReadOnlyList<AiConversationMessage>? conversationHistory)
    {
        if (conversationHistory is null || conversationHistory.Count == 0)
        {
            return question;
        }

        var normalized = Normalize(question);
        if (!LooksLikeFollowUp(normalized))
        {
            return question;
        }

        var recentContext = conversationHistory
            .TakeLast(4)
            .Select(message => message.Content)
            .Where(content => !string.IsNullOrWhiteSpace(content))
            .ToArray();

        if (recentContext.Length == 0)
        {
            return question;
        }

        return $"{question} {string.Join(' ', recentContext)}";
    }

    private static double Score(string normalizedQuestion, IReadOnlyList<string> questionTokens, KnowledgeEntry entry)
    {
        var keywordPhrases = entry.Keywords
            .Append(entry.Title)
            .Append(entry.CategoryTitle)
            .Where(keyword => !string.IsNullOrWhiteSpace(keyword))
            .Select(Normalize)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (keywordPhrases.Length == 0)
        {
            return 0;
        }

        var phraseHits = keywordPhrases.Count(keyword => normalizedQuestion.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        var phraseScore = (double)phraseHits / keywordPhrases.Length;

        var keywordTokens = keywordPhrases
            .SelectMany(Tokenize)
            .Where(token => token.Length > 1)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (keywordTokens.Length == 0)
        {
            return 0;
        }

        var overlapCount = questionTokens.Intersect(keywordTokens, StringComparer.OrdinalIgnoreCase).Count();
        var overlapScore = (double)overlapCount / Math.Max(1, questionTokens.Count);

        var fuzzyHits = 0;
        foreach (var token in questionTokens.Where(x => x.Length >= 4))
        {
            if (keywordTokens.Any(keywordToken =>
                keywordToken.StartsWith(token, StringComparison.OrdinalIgnoreCase)
                || token.StartsWith(keywordToken, StringComparison.OrdinalIgnoreCase)
                || LevenshteinDistance(token, keywordToken) <= 1))
            {
                fuzzyHits++;
            }
        }

        var fuzzyScore = (double)fuzzyHits / Math.Max(1, questionTokens.Count);
        var titleBoost = normalizedQuestion.Contains(Normalize(entry.Title), StringComparison.OrdinalIgnoreCase) ? 0.18 : 0;
        var contentBoost = normalizedQuestion.Contains(Normalize(entry.Content).Split(' ').FirstOrDefault() ?? string.Empty, StringComparison.OrdinalIgnoreCase) ? 0.04 : 0;
        var intentBoost = ComputeIntentBoost(normalizedQuestion, entry);

        return (phraseScore * 0.44) + (overlapScore * 0.32) + (fuzzyScore * 0.16) + titleBoost + contentBoost + intentBoost;
    }

    private static double ComputeIntentBoost(string normalizedQuestion, KnowledgeEntry entry)
    {
        var allEntryText = string.Join(" ", entry.Keywords.Append(entry.Title).Append(entry.CategoryTitle)).ToLowerInvariant();
        var score = 0.0;

        foreach (var (_, synonyms) in IntentSynonyms)
        {
            var queryMatchesIntent = synonyms.Any(synonym => normalizedQuestion.Contains(Normalize(synonym), StringComparison.OrdinalIgnoreCase));
            if (!queryMatchesIntent)
            {
                continue;
            }

            var entryMatchesIntent = synonyms.Any(synonym => allEntryText.Contains(Normalize(synonym), StringComparison.OrdinalIgnoreCase));
            if (entryMatchesIntent)
            {
                score += 0.08;
            }
        }

        return Math.Min(score, 0.24);
    }

    private static int LevenshteinDistance(string left, string right)
    {
        if (left == right)
        {
            return 0;
        }

        if (left.Length == 0)
        {
            return right.Length;
        }

        if (right.Length == 0)
        {
            return left.Length;
        }

        var matrix = new int[left.Length + 1, right.Length + 1];

        for (var i = 0; i <= left.Length; i++)
        {
            matrix[i, 0] = i;
        }

        for (var j = 0; j <= right.Length; j++)
        {
            matrix[0, j] = j;
        }

        for (var i = 1; i <= left.Length; i++)
        {
            for (var j = 1; j <= right.Length; j++)
            {
                var cost = left[i - 1] == right[j - 1] ? 0 : 1;
                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost);
            }
        }

        return matrix[left.Length, right.Length];
    }

    private static IReadOnlyList<string> Tokenize(string value)
    {
        var normalized = Normalize(value);
        return normalized
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(token => token.Length > 1)
            .ToArray();
    }

    private static string Normalize(string value)
    {
        var lowered = (value ?? string.Empty).Trim().ToLowerInvariant();
        lowered = PunctuationRegex.Replace(lowered, " ");
        return MultiSpaceRegex.Replace(lowered, " ");
    }
}
