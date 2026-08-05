using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MamiaSeedsOil.Web.Configuration;

namespace MamiaSeedsOil.Web.Controllers;

[Route("localization")]
public sealed class LocalizationController : Controller
{
    private readonly SiteLocalizationOptions _localizationOptions;

    public LocalizationController(IOptions<SiteLocalizationOptions> localizationOptions)
    {
        _localizationOptions = localizationOptions.Value;
    }

    [HttpGet("set")]
    public IActionResult SetLanguage(string culture, string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(culture)
            || !_localizationOptions.SupportedCultures.Contains(culture, StringComparer.OrdinalIgnoreCase))
        {
            culture = _localizationOptions.DefaultCulture;
        }

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            });

        var safeUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;
        return LocalRedirect(safeUrl);
    }
}
