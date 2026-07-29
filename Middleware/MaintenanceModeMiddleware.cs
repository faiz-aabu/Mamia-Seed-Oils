using MamiaSeedsOil.Web.Configuration;
using Microsoft.Extensions.Options;

namespace MamiaSeedsOil.Web.Middleware;

public sealed class MaintenanceModeMiddleware : IMiddleware
{
    private readonly SiteOperationsOptions _options;

    public MaintenanceModeMiddleware(IOptions<SiteOperationsOptions> options)
    {
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!_options.MaintenanceModeEnabled)
        {
            await next(context);
            return;
        }

        var path = context.Request.Path.Value ?? string.Empty;
        if (path.StartsWith("/error", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/health", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/status", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/css", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/js", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/images", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/api/ai-assistant/suggestions", StringComparison.OrdinalIgnoreCase))
        {
            await next(context);
            return;
        }

        context.Response.Redirect("/error/maintenance");
    }
}
