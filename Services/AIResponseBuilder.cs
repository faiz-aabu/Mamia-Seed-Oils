using MamiaSeedsOil.Web.Configuration;
using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Models.AiAssistant;
using MamiaSeedsOil.Web.Models.KnowledgeArchitecture;
using Microsoft.Extensions.Options;

namespace MamiaSeedsOil.Web.Services;

public sealed class AIResponseBuilder : IAIResponseBuilder
{
    private const string EstablishmentDateResponse = "Mamia Seeds Oil Limited was established on 16 July 2005.";
    private const string UnknownResponse = "I don't have that information at the moment. Please contact our team through the Contact page for further assistance.";
    private readonly AiAssistantOptions _options;

    public AIResponseBuilder(IOptions<AiAssistantOptions> options)
    {
        _options = options.Value;
    }

    public AiAnswerResult Build(AiQueryContext context, IReadOnlyList<string> suggestedQuestions)
    {
        if (IsEstablishmentDateQuestion(context.Question))
        {
            return new AiAnswerResult
            {
                Message = EstablishmentDateResponse,
                IsFallback = false,
                Suggestions = suggestedQuestions.Take(3).ToList()
            };
        }

        if (!context.IsDomainQuestion)
        {
            return new AiAnswerResult
            {
                Message = UnknownResponse,
                IsFallback = true,
                Suggestions = suggestedQuestions.Take(3).ToList()
            };
        }

        if (context.BestMatch is null || context.BestMatch.IsUnavailable)
        {
            return new AiAnswerResult
            {
                Message = UnknownResponse,
                IsFallback = true,
                Suggestions = suggestedQuestions.Take(4).ToList()
            };
        }

        return new AiAnswerResult
        {
            Message = context.BestMatch.Content,
            IsFallback = false,
            Suggestions = suggestedQuestions
                .Where(question => !string.Equals(question, context.Question, StringComparison.OrdinalIgnoreCase))
                .Take(3)
                .ToList()
        };
    }

    private static bool IsEstablishmentDateQuestion(string question)
    {
        var normalized = (question ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return false;
        }

        var referencesCompany = normalized.Contains("mamia")
            || normalized.Contains("company");

        if (!referencesCompany)
        {
            return false;
        }

        return normalized.Contains("established")
            || normalized.Contains("founded")
            || normalized.Contains("how old");
    }
}
