using MamiaSeedsOil.Web.Models.Knowledge;

namespace MamiaSeedsOil.Web.Interfaces;

public interface IKnowledgeRepository
{
    Task<CompanyKnowledgeModel> GetAsync(CancellationToken cancellationToken = default);
}
