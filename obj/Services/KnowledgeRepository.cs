using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Models.Knowledge;

namespace MamiaSeedsOil.Web.Services;

public sealed class KnowledgeRepository : IKnowledgeRepository
{
    private readonly IKnowledgeLoader _loader;
    private readonly IKnowledgeValidator _validator;
    private readonly ILogger<KnowledgeRepository> _logger;
    private readonly SemaphoreSlim _mutex = new(1, 1);
    private CompanyKnowledgeModel? _cache;

    public KnowledgeRepository(IKnowledgeLoader loader, IKnowledgeValidator validator, ILogger<KnowledgeRepository> logger)
    {
        _loader = loader;
        _validator = validator;
        _logger = logger;
    }

    public async Task<CompanyKnowledgeModel> GetAsync(CancellationToken cancellationToken = default)
    {
        if (_cache is not null)
        {
            return _cache;
        }

        await _mutex.WaitAsync(cancellationToken);
        try
        {
            if (_cache is not null)
            {
                return _cache;
            }

            var model = await _loader.LoadAsync(cancellationToken);
            var validation = _validator.Validate(model);

            if (!validation.IsValid)
            {
                _logger.LogWarning("Knowledge base loaded with warnings: {Warnings}", string.Join(" | ", validation.Warnings));
            }

            _cache = model;
            return _cache;
        }
        finally
        {
            _mutex.Release();
        }
    }
}
