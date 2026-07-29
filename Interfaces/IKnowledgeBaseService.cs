using MamiaSeedsOil.Web.Models.Knowledge;

namespace MamiaSeedsOil.Web.Interfaces;

public interface IKnowledgeBaseService
{
    Task<CompanyKnowledgeModel> GetKnowledgeAsync(CancellationToken cancellationToken = default);
}
