using MamiaSeedsOil.Web.Configuration;
using MamiaSeedsOil.Web.DTOs.AiAssistant;
using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Models.AiAssistant;
using MamiaSeedsOil.Web.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MamiaSeedsOil.Web.Services;

public sealed class AiAssistantService : IAiAssistantService
{
    private const int MaxConversationTurns = 12;
    private static readonly TimeSpan ConversationTtl = TimeSpan.FromMinutes(45);
    private readonly IAiProviderFactory _providerFactory;
    private readonly AiAssistantOptions _options;
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly ILogger<AiAssistantService> _logger;
    private static readonly ConcurrentDictionary<string, ConversationState> ConversationStore = new(StringComparer.Ordinal);
    private static readonly Regex EmailRegex = new(@"[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex LongDigitRegex = new(@"\b\d{7,}\b", RegexOptions.Compiled);

    public AiAssistantService(
        IAiProviderFactory providerFactory,
        IOptions<AiAssistantOptions> options,
        IStringLocalizer<SharedResource> localizer,
        ILogger<AiAssistantService> logger)
    {
        _providerFactory = providerFactory;
        _options = options.Value;
        _localizer = localizer;
        _logger = logger;
    }

    public async Task<AiChatResponseDto> GetResponseAsync(AiChatRequestDto request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var provider = _providerFactory.ResolveProvider();
        var safeQuestion = RedactPersonalData(request.Message);
        var conversationId = string.IsNullOrWhiteSpace(request.ConversationId)
            ? Guid.NewGuid().ToString("N")
            : request.ConversationId!;

        CleanupExpiredConversations();
        var state = ConversationStore.GetOrAdd(conversationId, _ => new ConversationState());
        var historySnapshot = state.GetHistorySnapshot();

        _logger.LogInformation("AI assistant question received. Provider={Provider}; Question={Question}", provider.ProviderName, safeQuestion);

        try
        {
            var result = await provider.GenerateAnswerAsync(request.Message, historySnapshot, cancellationToken);
            stopwatch.Stop();

            if (result.IsFallback)
            {
                _logger.LogInformation("AI assistant unknown or unavailable question. Provider={Provider}; DurationMs={DurationMs}", provider.ProviderName, stopwatch.ElapsedMilliseconds);
            }

            _logger.LogInformation("AI assistant response completed. Provider={Provider}; DurationMs={DurationMs}", provider.ProviderName, stopwatch.ElapsedMilliseconds);

            state.AddMessage("user", request.Message);
            state.AddMessage("assistant", result.Message);

            return new AiChatResponseDto
            {
                ConversationId = conversationId,
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

        var suggestions = new[]
        {
            L("AiSuggestedDistributor", "Become a Distributor"),
            L("AiSuggestedProducts", "View Products"),
            L("AiSuggestedBulk", "Bulk Orders"),
            L("AiSuggestedFactory", "Factory Location"),
            L("AiSuggestedSales", "Contact Sales"),
            L("AiSuggestedSoybeanMeal", "Soybean Meal"),
            L("AiSuggestedCookingOil", "Cooking Oil"),
            L("AiSuggestedCertifications", "Certifications")
        };

        return Task.FromResult(new AiSuggestionResponseDto
        {
            Suggestions = suggestions
        });
    }

    public async IAsyncEnumerable<AiStreamChunkDto> StreamResponseAsync(AiChatRequestDto request, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var provider = _providerFactory.ResolveProvider();
        var conversationId = string.IsNullOrWhiteSpace(request.ConversationId)
            ? Guid.NewGuid().ToString("N")
            : request.ConversationId!;
        var state = ConversationStore.GetOrAdd(conversationId, _ => new ConversationState());
        var historySnapshot = state.GetHistorySnapshot();
        var aggregate = new System.Text.StringBuilder();

        await foreach (var chunk in provider.StreamAnswerAsync(request.Message, historySnapshot, cancellationToken))
        {
            aggregate.Append(chunk);
            yield return new AiStreamChunkDto
            {
                Type = "chunk",
                Content = chunk,
                IsFinal = false
            };
        }

        state.AddMessage("user", request.Message);
        state.AddMessage("assistant", aggregate.ToString().Trim());

        yield return new AiStreamChunkDto
        {
            Type = "done",
            Content = string.Empty,
            IsFinal = true
        };
    }

    private static void CleanupExpiredConversations()
    {
        var threshold = DateTimeOffset.UtcNow.Subtract(ConversationTtl);
        foreach (var pair in ConversationStore)
        {
            if (pair.Value.LastUpdatedUtc < threshold)
            {
                ConversationStore.TryRemove(pair.Key, out _);
            }
        }
    }

    private static string RedactPersonalData(string text)
    {
        var value = text ?? string.Empty;
        value = EmailRegex.Replace(value, "[redacted-email]");
        value = LongDigitRegex.Replace(value, "[redacted-number]");
        return value.Length > 300 ? value[..300] : value;
    }

    private string L(string key, string fallback)
    {
        var value = _localizer[key];
        return value.ResourceNotFound ? fallback : value.Value;
    }

    private sealed class ConversationState
    {
        private readonly object _lock = new();
        private readonly List<AiConversationMessage> _messages = [];

        public DateTimeOffset LastUpdatedUtc { get; private set; } = DateTimeOffset.UtcNow;

        public void AddMessage(string role, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return;
            }

            lock (_lock)
            {
                _messages.Add(new AiConversationMessage
                {
                    Role = role,
                    Content = content.Trim(),
                    Timestamp = DateTimeOffset.UtcNow
                });

                if (_messages.Count > MaxConversationTurns)
                {
                    _messages.RemoveRange(0, _messages.Count - MaxConversationTurns);
                }

                LastUpdatedUtc = DateTimeOffset.UtcNow;
            }
        }

        public IReadOnlyList<AiConversationMessage> GetHistorySnapshot()
        {
            lock (_lock)
            {
                return _messages
                    .Select(message => new AiConversationMessage
                    {
                        Role = message.Role,
                        Content = message.Content,
                        Timestamp = message.Timestamp
                    })
                    .ToList();
            }
        }
    }
}
