using MamiaSeedsOil.Web.Models;

namespace MamiaSeedsOil.Web.Interfaces;

public interface IProductCatalogService
{
    Task<IReadOnlyList<ProductCatalogItem>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
}
