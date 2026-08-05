using MamiaSeedsOil.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MamiaSeedsOil.Web.Controllers;

public class HomeController : Controller
{
    private readonly IWebsiteContentService _websiteContentService;
    private readonly ISeoService _seoService;

    public HomeController(IWebsiteContentService websiteContentService, ISeoService seoService)
    {
        _websiteContentService = websiteContentService;
        _seoService = seoService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = await _websiteContentService.GetHomePageContentAsync(cancellationToken);
        ViewData["AssetPlaceholders"] = model.Placeholders;
        _seoService.ApplySeoMetadata(model.Seo, ViewData);
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        ViewData["CanonicalUrl"] = $"{baseUrl}{Request.Path}";
        ViewData["OrganizationJsonLd"] = _seoService.BuildOrganizationJsonLd(model);
        ViewData["StructuredDataJsonLd"] = _seoService.BuildStructuredDataJsonLd(model);

        return View(model);
    }
}
