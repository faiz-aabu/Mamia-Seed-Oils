using MamiaSeedsOil.Web.Configuration;
using MamiaSeedsOil.Web.Resources;
using MamiaSeedsOil.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace MamiaSeedsOil.Web.ViewComponents;

public sealed class AiAssistantWidgetViewComponent : ViewComponent
{
    private readonly AiAssistantOptions _options;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public AiAssistantWidgetViewComponent(
        IOptions<AiAssistantOptions> options,
        IStringLocalizer<SharedResource> localizer)
    {
        _options = options.Value;
        _localizer = localizer;
    }

    public IViewComponentResult Invoke()
    {
        var localizedSuggestions = new[]
        {
            L("AiSuggestedDistributor", "Become a Distributor"),
            L("AiSuggestedProducts", "View Products"),
            L("AiSuggestedBulk", "Bulk Orders"),
            L("AiSuggestedFactory", "Factory Location"),
            L("AiSuggestedSales", "Contact Sales"),
            L("AiSuggestedSoybeanMeal", "Soybean Meal"),
            L("AiSuggestedCookingOil", "Cooking Oil"),
            L("AiSuggestedCertifications", "Certifications")
        };

        var model = new AiAssistantWidgetViewModel
        {
            Enabled = _options.Enabled,
            DisplayName = _options.DisplayName,
            WelcomeMessage = L("AiWelcomeMessage", _options.WelcomeMessage),
            SuggestedQuestions = localizedSuggestions,
            ChatEndpoint = "/api/ai-assistant/chat",
            SuggestionsEndpoint = "/api/ai-assistant/suggestions"
        };

        return View(model);
    }

    private string L(string key, string fallback)
    {
        var value = _localizer[key];
        return value.ResourceNotFound ? fallback : value.Value;
    }
}
