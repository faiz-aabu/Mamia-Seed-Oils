using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MamiaSeedsOil.Web.Controllers;

public class HomeController : Controller
{
    private readonly IWebsiteContentService _websiteContentService;
    private readonly ISeoService _seoService;
    private readonly SiteLocalizationOptions _localizationOptions;

    public HomeController(IWebsiteContentService websiteContentService, ISeoService seoService, IOptions<SiteLocalizationOptions> localizationOptions)
    {
        _websiteContentService = websiteContentService;
        _seoService = seoService;
        _localizationOptions = localizationOptions.Value;
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
        ViewData["HreflangLinks"] = _seoService.BuildHreflangLinks(
            baseUrl,
            Request.Path,
            _localizationOptions.SupportedCultures,
            _localizationOptions.DefaultCulture);

        return View(model);
    }
}
