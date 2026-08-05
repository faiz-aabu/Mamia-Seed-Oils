using System.ComponentModel.DataAnnotations;

namespace MamiaSeedsOil.Web.Configuration;

public sealed class ProductCatalogOptions
{
    public const string SectionName = "ProductCatalog";

    [Required]
    public List<ProductCatalogItemOptions> Items { get; set; } = [];
}

public sealed class ProductCatalogItemOptions
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = string.Empty;

    [Required]
    public string PackagingSize { get; set; } = string.Empty;

    [Required]
    public string Image { get; set; } = string.Empty;

    public string? ImageWebp { get; set; }

    public bool IsPlaceholderImage { get; set; } = true;

    [Required]
    public string Availability { get; set; } = string.Empty;

    [Required]
    public string SeoUrl { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }

    [Required]
    public string Status { get; set; } = "Active";
}
