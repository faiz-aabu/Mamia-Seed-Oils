using MamiaSeedsOil.Web.Models.AiAssistant;

namespace MamiaSeedsOil.Web.Interfaces;

public interface IAiProvider
{
    string ProviderName { get; }
    Task<AiAnswerResult> GenerateAnswerAsync(
        string message,
        IReadOnlyList<AiConversationMessage>? conversationHistory = null,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<string> StreamAnswerAsync(
        string message,
        IReadOnlyList<AiConversationMessage>? conversationHistory = null,
        CancellationToken cancellationToken = default);
}
