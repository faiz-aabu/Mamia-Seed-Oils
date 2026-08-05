using MamiaSeedsOil.Web.Models.KnowledgeArchitecture;

namespace MamiaSeedsOil.Web.Interfaces;

public interface IKnowledgeService
{
    Task<IReadOnlyList<KnowledgeCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KnowledgeEntry>> GetEntriesAsync(CancellationToken cancellationToken = default);
    bool IsDomainQuestion(string question);
}
