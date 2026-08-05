using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Models.Knowledge;

namespace MamiaSeedsOil.Web.Services;

public sealed class AiKnowledgeBaseService : IAiKnowledgeBaseService
{
    private readonly IKnowledgeBaseService _knowledgeBaseService;

    public AiKnowledgeBaseService(IKnowledgeBaseService knowledgeBaseService)
    {
        _knowledgeBaseService = knowledgeBaseService;
    }

    public Task<CompanyKnowledgeModel> GetKnowledgeBaseAsync(CancellationToken cancellationToken = default)
    {
        return _knowledgeBaseService.GetKnowledgeAsync(cancellationToken);
    }
}
