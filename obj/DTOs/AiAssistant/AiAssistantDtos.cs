using System.ComponentModel.DataAnnotations;
using MamiaSeedsOil.Web.Validation;

namespace MamiaSeedsOil.Web.DTOs.AiAssistant;

public sealed class AiChatRequestDto
{
    [Required(ErrorMessage = "ValidationRequired")]
    [StringLength(800, ErrorMessage = "ValidationStringLength")]
    [NoHtml(ErrorMessage = "ValidationNoHtml")]
    public string Message { get; set; } = string.Empty;

    [StringLength(80, ErrorMessage = "ValidationStringLength")]
    public string? ConversationId { get; set; }
}

public sealed class AiChatResponseDto
{
    public string ConversationId { get; set; } = string.Empty;
    public string AgentName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }
    public bool IsFallback { get; set; }
    public IReadOnlyList<string> Suggestions { get; set; } = [];
}

public sealed class AiSuggestionResponseDto
{
    public IReadOnlyList<string> Suggestions { get; set; } = [];
}

public sealed class AiStreamChunkDto
{
    public string Type { get; set; } = "chunk";
    public string Content { get; set; } = string.Empty;
    public bool IsFinal { get; set; }
}
