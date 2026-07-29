using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MamiaSeedsOil.Web.Controllers;

[ApiController]
public sealed class SeoController : ControllerBase
{
    private readonly IWebsiteContentService _contentService;
    private readonly ISeoService _seoService;
    private readonly SiteLocalizationOptions _localizationOptions;

    public SeoController(IWebsiteContentService contentService, ISeoService seoService, IOptions<SiteLocalizationOptions> localizationOptions)
    {
        _contentService = contentService;
        _seoService = seoService;
        _localizationOptions = localizationOptions.Value;
    }

    [HttpGet("sitemap.xml")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Sitemap(CancellationToken cancellationToken)
    {
        var model = await _contentService.GetHomePageContentAsync(cancellationToken);
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var xml = _seoService.BuildSitemapXml(baseUrl, model, _localizationOptions.SupportedCultures, _localizationOptions.DefaultCulture);
        return Content(xml, "application/xml");
    }

    [HttpGet("robots.txt")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
    public IActionResult Robots()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var content = _seoService.BuildRobotsTxt($"{baseUrl}/sitemap.xml");
        return Content(content, "text/plain");
    }
}
