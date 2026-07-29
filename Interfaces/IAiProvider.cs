using MamiaSeedsOil.Web.Models.AiAssistant;

namespace MamiaSeedsOil.Web.Interfaces;

public interface IAiProvider
{
    string ProviderName { get; }
    Task<AiAnswerResult> GenerateAnswerAsync(string message, CancellationToken cancellationToken = default);
    IAsyncEnumerable<string> StreamAnswerAsync(string message, CancellationToken cancellationToken = default);
}
