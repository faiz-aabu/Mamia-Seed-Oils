namespace MamiaSeedsOil.Web.Interfaces;

public interface IImageUrlService
{
    string ResolveUrl(string? path);
    string ResolveSizes(string? sizes);
    string ResolveBlurPlaceholder();
}
