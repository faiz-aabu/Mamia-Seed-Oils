using MamiaSeedsOil.Web.Configuration;
using MamiaSeedsOil.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MamiaSeedsOil.Web.ViewComponents;

public sealed class AiAssistantWidgetViewComponent : ViewComponent
{
    private readonly AiAssistantOptions _options;

    public AiAssistantWidgetViewComponent(IOptions<AiAssistantOptions> options)
    {
        _options = options.Value;
    }

    public IViewComponentResult Invoke()
    {
        var model = new AiAssistantWidgetViewModel
        {
            Enabled = _options.Enabled,
            DisplayName = _options.DisplayName,
            WelcomeMessage = _options.WelcomeMessage,
            SuggestedQuestions = _options.SuggestedQuestions,
            ChatEndpoint = "/api/ai-assistant/chat",
            SuggestionsEndpoint = "/api/ai-assistant/suggestions"
        };

        return View(model);
    }
}
