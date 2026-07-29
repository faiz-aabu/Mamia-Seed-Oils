using MamiaSeedsOil.Web.Models.KnowledgeArchitecture;

namespace MamiaSeedsOil.Web.Interfaces;

public interface IAIContextBuilder
{
    Task<AiQueryContext> BuildAsync(string question, CancellationToken cancellationToken = default);
}
