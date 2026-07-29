namespace MamiaSeedsOil.Web.Models.AiAssistant;

public sealed class AiConversationMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }
}

public sealed class AiAnswerResult
{
    public string Message { get; set; } = string.Empty;
    public bool IsFallback { get; set; }
    public List<string> Suggestions { get; set; } = [];
}
