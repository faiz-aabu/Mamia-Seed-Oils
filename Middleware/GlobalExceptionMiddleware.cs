using MamiaSeedsOil.Web.Models.Api;
using System.Text.Json;

namespace MamiaSeedsOil.Web.Middleware;

public sealed class GlobalExceptionMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _hostEnvironment;

    public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment hostEnvironment)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception for request {Path}", context.Request.Path);

            if (context.Response.HasStarted)
            {
                return;
            }

            if (IsApiRequest(context.Request))
            {
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var payload = new ApiErrorResponse
                {
                    Message = "An internal server error occurred.",
                    StatusCode = StatusCodes.Status500InternalServerError,
                    TraceId = context.TraceIdentifier,
                    Detail = _hostEnvironment.IsDevelopment() ? exception.Message : null
                };

                var json = JsonSerializer.Serialize(payload);
                await context.Response.WriteAsync(json);
                return;
            }

            context.Response.Redirect("/error/500");
        }
    }

    private static bool IsApiRequest(HttpRequest request)
    {
        if (request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var acceptHeaders = request.GetTypedHeaders().Accept;
        if (acceptHeaders is null || acceptHeaders.Count == 0)
        {
            return false;
        }

        return acceptHeaders.Any(header =>
            header.MediaType.Value?.Contains("json", StringComparison.OrdinalIgnoreCase) == true);
    }
}
