using MamiaSeedsOil.Web.Configuration;
using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Models.AiAssistant;
using MamiaSeedsOil.Web.Models.KnowledgeArchitecture;
using MamiaSeedsOil.Web.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace MamiaSeedsOil.Web.Services;

public sealed class AIResponseBuilder : IAIResponseBuilder
{
    private readonly AiAssistantOptions _options;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public AIResponseBuilder(
        IOptions<AiAssistantOptions> options,
        IStringLocalizer<SharedResource> localizer)
    {
        _options = options.Value;
        _localizer = localizer;
    }

    public AiAnswerResult Build(AiQueryContext context, IReadOnlyList<string> suggestedQuestions)
    {
        if (!context.IsDomainQuestion)
        {
            return new AiAnswerResult
            {
                Message = UnknownResponse(),
                IsFallback = true,
                Suggestions = suggestedQuestions.Take(3).ToList()
            };
        }

        if (context.BestMatch is null || context.BestMatch.IsUnavailable)
        {
            return new AiAnswerResult
            {
                Message = UnknownResponse(),
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

    private string UnknownResponse()
    {
        var value = _localizer["AiUnknownResponse"];
        if (!value.ResourceNotFound)
        {
            return value.Value;
        }

        return "I don't have that information at the moment. Please contact our team through the Contact page for further assistance.";
    }
}
