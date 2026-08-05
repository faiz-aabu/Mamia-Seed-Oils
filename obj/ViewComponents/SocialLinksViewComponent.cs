using MamiaSeedsOil.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MamiaSeedsOil.Web.ViewComponents;

public sealed class SocialLinksViewComponent : ViewComponent
{
    private readonly IWebsiteContentService _websiteContentService;

    public SocialLinksViewComponent(IWebsiteContentService websiteContentService)
    {
        _websiteContentService = websiteContentService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = (await _websiteContentService.GetHomePageContentAsync()).Company.SocialLinks;
        return View(model);
    }
}
