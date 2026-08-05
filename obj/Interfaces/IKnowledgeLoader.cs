using MamiaSeedsOil.Web.Models.Knowledge;

namespace MamiaSeedsOil.Web.Interfaces;

public interface IKnowledgeLoader
{
    Task<CompanyKnowledgeModel> LoadAsync(CancellationToken cancellationToken = default);
}
