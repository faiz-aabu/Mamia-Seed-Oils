using MamiaSeedsOil.Web.Configuration;
using MamiaSeedsOil.Web.DTOs.AiAssistant;
using MamiaSeedsOil.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace MamiaSeedsOil.Web.Controllers;

[ApiController]
[Route("api/ai-assistant")]
public sealed class AiAssistantController : ControllerBase
{
    private readonly IAiAssistantService _assistantService;
    private readonly ILogger<AiAssistantController> _logger;
    private readonly FeatureFlagsOptions _featureFlags;

    public AiAssistantController(
        IAiAssistantService assistantService,
        ILogger<AiAssistantController> logger,
        IOptions<FeatureFlagsOptions> featureFlags)
    {
        _assistantService = assistantService;
        _logger = logger;
        _featureFlags = featureFlags.Value;
    }

    [HttpGet("suggestions")]
    public async Task<ActionResult<AiSuggestionResponseDto>> GetSuggestions(CancellationToken cancellationToken)
    {
        if (!_featureFlags.EnableAiAssistant)
        {
            return NotFound();
        }

        _logger.LogInformation("AI suggestions requested.");
        var response = await _assistantService.GetSuggestionsAsync(cancellationToken);
        return Ok(response);
    }

    [HttpPost("chat")]
    [EnableRateLimiting("AiAssistantPolicy")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult<AiChatResponseDto>> Chat([FromBody] AiChatRequestDto request, CancellationToken cancellationToken)
    {
        if (!_featureFlags.EnableAiAssistant)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            _logger.LogInformation("AI chat request accepted. ConversationId={ConversationId}", request.ConversationId);
            var response = await _assistantService.GetResponseAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("AI request canceled by client");
            return StatusCode(StatusCodes.Status499ClientClosedRequest);
        }
    }

    [HttpPost("stream")]
    [EnableRateLimiting("AiAssistantPolicy")]
    [ValidateAntiForgeryToken]
    public async Task Stream([FromBody] AiChatRequestDto request, CancellationToken cancellationToken)
    {
        if (!_featureFlags.EnableAiAssistant)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        if (!ModelState.IsValid)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            Response.ContentType = "application/json";
            await Response.WriteAsJsonAsync(new
            {
                message = "Validation failed.",
                statusCode = StatusCodes.Status400BadRequest,
                traceId = HttpContext.TraceIdentifier,
                errors = ModelState
                    .Where(kvp => kvp.Value?.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value!.Errors.Select(error => error.ErrorMessage).ToArray())
            }, cancellationToken);
            return;
        }

        Response.ContentType = "text/event-stream";
        _logger.LogInformation("AI stream request accepted. ConversationId={ConversationId}", request.ConversationId);

        await foreach (var chunk in _assistantService.StreamResponseAsync(request, cancellationToken))
        {
            var payload = System.Text.Json.JsonSerializer.Serialize(chunk);
            await Response.WriteAsync($"data: {payload}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }
}
