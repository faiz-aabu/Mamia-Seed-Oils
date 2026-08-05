using MamiaSeedsOil.Web.Configuration;
using MamiaSeedsOil.Web.Interfaces;
using Microsoft.Extensions.Options;

namespace MamiaSeedsOil.Web.Services;

public sealed class ImageUrlService : IImageUrlService
{
    private readonly ImageDeliveryOptions _options;

    public ImageUrlService(IOptions<ImageDeliveryOptions> options)
    {
        _options = options.Value;
    }

    public string ResolveUrl(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        if (!_options.UseCdn || string.IsNullOrWhiteSpace(_options.CdnBaseUrl))
        {
            return path;
        }

        return $"{_options.CdnBaseUrl.TrimEnd('/')}/{path.TrimStart('/')}";
    }

    public string ResolveSizes(string? sizes)
    {
        return string.IsNullOrWhiteSpace(sizes) ? _options.DefaultSizes : sizes;
    }

    public string ResolveBlurPlaceholder()
    {
        return _options.BlurPlaceholderDataUrl;
    }
}
