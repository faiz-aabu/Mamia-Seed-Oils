using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Models.Knowledge;

namespace MamiaSeedsOil.Web.Services;

public sealed class KnowledgeBaseService : IKnowledgeBaseService
{
    private readonly IKnowledgeRepository _repository;

    public KnowledgeBaseService(IKnowledgeRepository repository)
    {
        _repository = repository;
    }

    public Task<CompanyKnowledgeModel> GetKnowledgeAsync(CancellationToken cancellationToken = default)
    {
        return _repository.GetAsync(cancellationToken);
    }
}
