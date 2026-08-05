using MamiaSeedsOil.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MamiaSeedsOil.Web.Controllers;

[ApiController]
public sealed class SeoController : ControllerBase
{
    private readonly IWebsiteContentService _contentService;
    private readonly ISeoService _seoService;

    public SeoController(IWebsiteContentService contentService, ISeoService seoService)
    {
        _contentService = contentService;
        _seoService = seoService;
    }

    [HttpGet("sitemap.xml")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Sitemap(CancellationToken cancellationToken)
    {
        var model = await _contentService.GetHomePageContentAsync(cancellationToken);
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var xml = _seoService.BuildSitemapXml(baseUrl, model);
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
