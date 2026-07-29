using MamiaSeedsOil.Web.DTOs.AiAssistant;

namespace MamiaSeedsOil.Web.Interfaces;

public interface IAiAssistantService
{
    Task<AiChatResponseDto> GetResponseAsync(AiChatRequestDto request, CancellationToken cancellationToken = default);
    Task<AiSuggestionResponseDto> GetSuggestionsAsync(CancellationToken cancellationToken = default);
    IAsyncEnumerable<AiStreamChunkDto> StreamResponseAsync(AiChatRequestDto request, CancellationToken cancellationToken = default);
}
