using MamiaSeedsOil.Web.Configuration;
using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Models.AiAssistant;
using Microsoft.Extensions.Options;

namespace MamiaSeedsOil.Web.Services;

public sealed class RuleBasedAiProvider : IAiProvider
{
    private readonly AiAssistantOptions _options;
    private readonly IAIContextBuilder _contextBuilder;
    private readonly IAIResponseBuilder _responseBuilder;
    private readonly ILogger<RuleBasedAiProvider> _logger;

    public RuleBasedAiProvider(
        IOptions<AiAssistantOptions> options,
        IAIContextBuilder contextBuilder,
        IAIResponseBuilder responseBuilder,
        ILogger<RuleBasedAiProvider> logger)
    {
        _options = options.Value;
        _contextBuilder = contextBuilder;
        _responseBuilder = responseBuilder;
        _logger = logger;
    }

    public string ProviderName => "RuleBased";

    public async Task<AiAnswerResult> GenerateAnswerAsync(string message, CancellationToken cancellationToken = default)
    {
        var context = await _contextBuilder.BuildAsync(message, cancellationToken);
        var response = _responseBuilder.Build(context, _options.SuggestedQuestions);

        if (context.UnknownReason != Models.KnowledgeArchitecture.UnknownQuestionReason.None)
        {
            _logger.LogInformation("AI unknown question classification: {Reason}", context.UnknownReason);
        }

        return response;
    }

    public async IAsyncEnumerable<string> StreamAnswerAsync(string message, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var result = await GenerateAnswerAsync(message, cancellationToken);
        var words = result.Message.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in words)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return word + " ";
            await Task.Delay(18, cancellationToken);
        }
    }
}
