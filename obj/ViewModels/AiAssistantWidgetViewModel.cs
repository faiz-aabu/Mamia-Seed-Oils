namespace MamiaSeedsOil.Web.ViewModels;

public sealed class AiAssistantWidgetViewModel
{
    public bool Enabled { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string WelcomeMessage { get; init; } = string.Empty;
    public IReadOnlyList<string> SuggestedQuestions { get; init; } = [];
    public string ChatEndpoint { get; init; } = "/api/ai-assistant/chat";
    public string SuggestionsEndpoint { get; init; } = "/api/ai-assistant/suggestions";
}
