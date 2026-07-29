using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace MamiaSeedsOil.Web.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseWebPipeline(this WebApplication app)
    {
        var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
        app.UseRequestLocalization(localizationOptions);

        app.UseResponseCompression();
        app.UseCookiePolicy();
        app.UseMiddleware<MamiaSeedsOil.Web.Middleware.SecurityHeadersMiddleware>();
        app.UseMiddleware<MamiaSeedsOil.Web.Middleware.RequestTelemetryMiddleware>();
        app.UseMiddleware<MamiaSeedsOil.Web.Middleware.MaintenanceModeMiddleware>();
        app.UseMiddleware<MamiaSeedsOil.Web.Middleware.GlobalExceptionMiddleware>();

        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = context =>
            {
                var extension = Path.GetExtension(context.File.Name);
                var isHtml = string.Equals(extension, ".html", StringComparison.OrdinalIgnoreCase);

                if (isHtml)
                {
                    context.Context.Response.Headers.CacheControl = "no-cache,no-store,must-revalidate";
                    context.Context.Response.Headers.Pragma = "no-cache";
                    context.Context.Response.Headers.Expires = "0";
                    return;
                }

                var maxAge = app.Environment.IsDevelopment() ? TimeSpan.FromMinutes(5) : TimeSpan.FromDays(30);
                context.Context.Response.Headers.CacheControl = $"public,max-age={(int)maxAge.TotalSeconds},immutable";
            },
            FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "wwwroot"))
        });

        app.UseStatusCodePagesWithReExecute("/error/{0}");
        app.UseRouting();
        app.UseResponseCaching();
        app.UseRateLimiter();
        app.UseAuthorization();

        return app;
    }

    public static WebApplication MapWebRoutes(this WebApplication app)
    {
        app.MapControllers();

        app.MapControllerRoute(
            name: "localized-default",
            pattern: "{culture:regex(^en|ha$)}/{controller=Home}/{action=Index}/{id?}");

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            ResponseWriter = WriteHealthResponse
        });
        app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains("live"),
            ResponseWriter = WriteHealthResponse
        });
        app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains("ready"),
            ResponseWriter = WriteHealthResponse
        });

        return app;
    }

    private static Task WriteHealthResponse(HttpContext context, Microsoft.Extensions.Diagnostics.HealthChecks.HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var payload = new
        {
            status = report.Status.ToString(),
            totalDurationMs = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                durationMs = entry.Value.Duration.TotalMilliseconds
            }),
            traceId = context.TraceIdentifier
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
