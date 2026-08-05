using MamiaSeedsOil.Web.Models.Knowledge;

namespace MamiaSeedsOil.Web.Interfaces;

public interface IAiKnowledgeBaseService
{
    Task<CompanyKnowledgeModel> GetKnowledgeBaseAsync(CancellationToken cancellationToken = default);
}
