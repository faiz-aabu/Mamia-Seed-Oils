using MamiaSeedsOil.Web.Models.Knowledge;

namespace MamiaSeedsOil.Web.Interfaces;

public interface IKnowledgeSearchService
{
    Task<KnowledgeSearchResult> SearchAsync(string query, CompanyKnowledgeModel knowledgeBase, CancellationToken cancellationToken = default);
}
