namespace MamiaSeedsOil.Web.Models;

public sealed class ProductCatalogItem
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string PackagingSize { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string? ImageWebp { get; set; }
    public bool IsPlaceholderImage { get; set; } = true;
    public string Availability { get; set; } = string.Empty;
    public string SeoUrl { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public ProductCatalogStatus Status { get; set; }
}

public enum ProductCatalogStatus
{
    Inactive = 0,
    Active = 1
}
