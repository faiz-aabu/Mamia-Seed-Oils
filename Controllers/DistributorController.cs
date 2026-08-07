using MamiaSeedsOil.Web.DTOs;
using MamiaSeedsOil.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MamiaSeedsOil.Web.Controllers;

public sealed class DistributorController : Controller
{
    private readonly IDistributorApplicationService _distributorApplicationService;
    private readonly IWebsiteContentService _websiteContentService;
    private readonly ISeoService _seoService;
    private readonly ILogger<DistributorController> _logger;

    public DistributorController(IDistributorApplicationService distributorApplicationService, IWebsiteContentService websiteContentService, ISeoService seoService, ILogger<DistributorController> logger)
    {
        _distributorApplicationService = distributorApplicationService;
        _websiteContentService = websiteContentService;
        _seoService = seoService;
        _logger = logger;
    }

    [HttpGet("/become-distributor")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = await _websiteContentService.GetHomePageContentAsync(cancellationToken);
        _logger.LogInformation("✔ Distributor form received");
        ViewData["Title"] = "Become an Authorized Distributor | Mamia Seeds Oil Limited";
        ViewData["SeoDescription"] = "Apply to become an authorized distributor of Mamia Seeds Oil Limited and grow with premium soybean products across Nigeria.";
        ViewData["SeoKeywords"] = "become distributor, distributor application, Mamia Seeds Oil distributor, soybean distributor";
        ViewData["CanonicalUrl"] = Url.Action(nameof(Index), "Distributor", values: null, protocol: Request.Scheme);
        ViewData["AssetPlaceholders"] = model.Placeholders;
        ViewData["HomePageModel"] = model;
        _seoService.ApplySeoMetadata(model.Seo, ViewData);

        return View(new DistributorApplicationRequestDto { Country = "Nigeria" });
    }

    [HttpPost("/become-distributor")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(DistributorApplicationRequestDto model, CancellationToken cancellationToken)
    {
        var pageModel = await _websiteContentService.GetHomePageContentAsync(cancellationToken);
        ViewData["AssetPlaceholders"] = pageModel.Placeholders;
        ViewData["Title"] = "Become an Authorized Distributor | Mamia Seeds Oil Limited";
        ViewData["SeoDescription"] = "Apply to become an authorized distributor of Mamia Seeds Oil Limited and grow with premium soybean products across Nigeria.";
        ViewData["SeoKeywords"] = "become distributor, distributor application, Mamia Seeds Oil distributor, soybean distributor";
        ViewData["CanonicalUrl"] = Url.Action(nameof(Index), "Distributor", values: null, protocol: Request.Scheme);
        ViewData["HomePageModel"] = pageModel;
        _seoService.ApplySeoMetadata(pageModel.Seo, ViewData);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("✘ Model validation failed for distributor application submission.");
            return View(model);
        }

        _logger.LogInformation("✔ Model validation passed");
        var result = await _distributorApplicationService.SubmitApplicationAsync(model, cancellationToken);
        if (!result.Success)
        {
            _logger.LogWarning("✘ Distributor application submission did not complete successfully. Message={Message}", result.Message);
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        _logger.LogInformation("✔ Distributor application submitted successfully. Message={Message}", result.Message);
        TempData["DistributorApplicationSuccess"] = result.Message;
        return RedirectToAction(nameof(Success));
    }

    [HttpGet("/become-distributor/success")]
    public async Task<IActionResult> Success(CancellationToken cancellationToken)
    {
        var model = await _websiteContentService.GetHomePageContentAsync(cancellationToken);
        ViewData["Title"] = "Application Submitted Successfully | Mamia Seeds Oil Limited";
        ViewData["SeoDescription"] = "Your distributor application has been received successfully.";
        ViewData["CanonicalUrl"] = Url.Action(nameof(Success), "Distributor", values: null, protocol: Request.Scheme);
        ViewData["AssetPlaceholders"] = model.Placeholders;
        ViewData["HomePageModel"] = model;
        _seoService.ApplySeoMetadata(model.Seo, ViewData);
        return View();
    }
}
