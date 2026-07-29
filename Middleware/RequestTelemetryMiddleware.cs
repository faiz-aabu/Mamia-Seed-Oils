using System.Diagnostics;
using MamiaSeedsOil.Web.Configuration;
using Microsoft.Extensions.Options;

namespace MamiaSeedsOil.Web.Middleware;

public sealed class RequestTelemetryMiddleware : IMiddleware
{
    private readonly ILogger<RequestTelemetryMiddleware> _logger;
    private readonly ObservabilityOptions _options;

    public RequestTelemetryMiddleware(ILogger<RequestTelemetryMiddleware> logger, IOptions<ObservabilityOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (_options.EnableRequestStartLogs)
        {
            _logger.LogInformation("Request started. Method={Method}; Path={Path}; TraceId={TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await next(context);
            stopwatch.Stop();

            var statusCode = context.Response.StatusCode;
            if (stopwatch.ElapsedMilliseconds >= _options.SlowRequestThresholdMs)
            {
                _logger.LogWarning("Performance warning: slow request detected. Method={Method}; Path={Path}; StatusCode={StatusCode}; DurationMs={DurationMs}; TraceId={TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    statusCode,
                    stopwatch.ElapsedMilliseconds,
                    context.TraceIdentifier);
            }
            else
            {
                _logger.LogInformation("Request completed. Method={Method}; Path={Path}; StatusCode={StatusCode}; DurationMs={DurationMs}; TraceId={TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    statusCode,
                    stopwatch.ElapsedMilliseconds,
                    context.TraceIdentifier);
            }
        }
        catch
        {
            stopwatch.Stop();
            _logger.LogError("Request failed. Method={Method}; Path={Path}; DurationMs={DurationMs}; TraceId={TraceId}",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds,
                context.TraceIdentifier);
            throw;
        }
    }
}
