using MamiaSeedsOil.Web.Models.AiAssistant;
using MamiaSeedsOil.Web.Models.KnowledgeArchitecture;

namespace MamiaSeedsOil.Web.Interfaces;

public interface IAIContextBuilder
{
    Task<AiQueryContext> BuildAsync(
        string question,
        IReadOnlyList<AiConversationMessage>? conversationHistory = null,
        CancellationToken cancellationToken = default);
}
