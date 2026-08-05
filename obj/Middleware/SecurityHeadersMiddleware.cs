using MamiaSeedsOil.Web.Configuration;
using Microsoft.Extensions.Options;

namespace MamiaSeedsOil.Web.Middleware;

public sealed class SecurityHeadersMiddleware : IMiddleware
{
    private readonly SecurityHeadersOptions _options;

    public SecurityHeadersMiddleware(IOptions<SecurityHeadersOptions> options)
    {
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (_options.Enabled)
        {
            var headers = context.Response.Headers;
            headers["X-Permitted-Cross-Domain-Policies"] = "none";
            headers["X-XSS-Protection"] = "0";

            if (_options.EnableXContentTypeOptions)
            {
                headers["X-Content-Type-Options"] = "nosniff";
            }

            if (_options.EnableFrameDeny)
            {
                headers["X-Frame-Options"] = "DENY";
            }

            if (_options.EnableReferrerPolicy)
            {
                headers["Referrer-Policy"] = _options.ReferrerPolicy;
            }

            if (_options.EnablePermissionsPolicy)
            {
                headers["Permissions-Policy"] = _options.PermissionsPolicy;
            }

            if (_options.EnableCrossOriginPolicies)
            {
                headers["Cross-Origin-Opener-Policy"] = _options.CrossOriginOpenerPolicy;
                headers["Cross-Origin-Resource-Policy"] = _options.CrossOriginResourcePolicy;
                headers["Cross-Origin-Embedder-Policy"] = _options.CrossOriginEmbedderPolicy;
            }

            if (_options.EnableCsp)
            {
                headers["Content-Security-Policy"] = _options.ContentSecurityPolicy;
            }
        }

        await next(context);
    }
}
