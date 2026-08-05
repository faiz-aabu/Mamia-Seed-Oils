using MamiaSeedsOil.Web.Configuration;
using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Models;
using Microsoft.Extensions.Options;

namespace MamiaSeedsOil.Web.Services;

public sealed class ProductCatalogService : IProductCatalogService
{
    private readonly ProductCatalogOptions _options;

    public ProductCatalogService(IOptions<ProductCatalogOptions> options)
    {
        _options = options.Value;
    }

    public Task<IReadOnlyList<ProductCatalogItem>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var items = _options.Items
            .Select(item => new ProductCatalogItem
            {
                Name = item.Name,
                Description = item.Description,
                Category = item.Category,
                PackagingSize = item.PackagingSize,
                Image = item.Image,
                ImageWebp = item.ImageWebp,
                IsPlaceholderImage = item.IsPlaceholderImage,
                Availability = item.Availability,
                SeoUrl = item.SeoUrl,
                DisplayOrder = item.DisplayOrder,
                Status = ParseStatus(item.Status)
            })
            .Where(item => item.Status == ProductCatalogStatus.Active)
            .OrderBy(item => item.DisplayOrder)
            .ToArray();

        return Task.FromResult<IReadOnlyList<ProductCatalogItem>>(items);
    }

    private static ProductCatalogStatus ParseStatus(string status)
    {
        return Enum.TryParse<ProductCatalogStatus>(status, ignoreCase: true, out var parsed)
            ? parsed
            : ProductCatalogStatus.Inactive;
    }
}
