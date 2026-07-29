using MamiaSeedsOil.Web.Configuration;
using MamiaSeedsOil.Web.DTOs.AiAssistant;
using MamiaSeedsOil.Web.Interfaces;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MamiaSeedsOil.Web.Services;

public sealed class AiAssistantService : IAiAssistantService
{
    private readonly IAiProviderFactory _providerFactory;
    private readonly AiAssistantOptions _options;
    private readonly ILogger<AiAssistantService> _logger;
    private static readonly Regex EmailRegex = new(@"[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex LongDigitRegex = new(@"\b\d{7,}\b", RegexOptions.Compiled);

    public AiAssistantService(
        IAiProviderFactory providerFactory,
        IOptions<AiAssistantOptions> options,
        ILogger<AiAssistantService> logger)
    {
        _providerFactory = providerFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<AiChatResponseDto> GetResponseAsync(AiChatRequestDto request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var provider = _providerFactory.ResolveProvider();
        var safeQuestion = RedactPersonalData(request.Message);

        _logger.LogInformation("AI assistant question received. Provider={Provider}; Question={Question}", provider.ProviderName, safeQuestion);

        try
        {
            var result = await provider.GenerateAnswerAsync(request.Message, cancellationToken);
            stopwatch.Stop();

            if (result.IsFallback)
            {
                _logger.LogInformation("AI assistant unknown or unavailable question. Provider={Provider}; DurationMs={DurationMs}", provider.ProviderName, stopwatch.ElapsedMilliseconds);
            }

            _logger.LogInformation("AI assistant response completed. Provider={Provider}; DurationMs={DurationMs}", provider.ProviderName, stopwatch.ElapsedMilliseconds);

            return new AiChatResponseDto
            {
                ConversationId = request.ConversationId ?? Guid.NewGuid().ToString("N"),
                AgentName = _options.DisplayName,
                Message = result.Message,
                Timestamp = DateTimeOffset.UtcNow,
                IsFallback = result.IsFallback,
                Suggestions = result.Suggestions
            };
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "AI assistant response failed. Provider={Provider}; DurationMs={DurationMs}", provider.ProviderName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    public Task<AiSuggestionResponseDto> GetSuggestionsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(new AiSuggestionResponseDto
        {
            Suggestions = _options.SuggestedQuestions
        });
    }

    public async IAsyncEnumerable<AiStreamChunkDto> StreamResponseAsync(AiChatRequestDto request, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var provider = _providerFactory.ResolveProvider();

        await foreach (var chunk in provider.StreamAnswerAsync(request.Message, cancellationToken))
        {
            yield return new AiStreamChunkDto
            {
                Type = "chunk",
                Content = chunk,
                IsFinal = false
            };
        }

        yield return new AiStreamChunkDto
        {
            Type = "done",
            Content = string.Empty,
            IsFinal = true
        };
    }

    private static string RedactPersonalData(string text)
    {
        var value = text ?? string.Empty;
        value = EmailRegex.Replace(value, "[redacted-email]");
        value = LongDigitRegex.Replace(value, "[redacted-number]");
        return value.Length > 300 ? value[..300] : value;
    }
}
