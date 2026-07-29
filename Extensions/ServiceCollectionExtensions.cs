using MamiaSeedsOil.Web.Configuration;
using MamiaSeedsOil.Web.Helpers;
using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Middleware;
using MamiaSeedsOil.Web.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Globalization;
using System.Threading.RateLimiting;

namespace MamiaSeedsOil.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<WebsiteContentOptions>()
            .Bind(configuration.GetSection(WebsiteContentOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<CompanyProfileOptions>()
            .Bind(configuration.GetSection(CompanyProfileOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<ProductCatalogOptions>()
            .Bind(configuration.GetSection(ProductCatalogOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<AiAssistantOptions>()
            .Bind(configuration.GetSection(AiAssistantOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<SiteLocalizationOptions>()
            .Bind(configuration.GetSection(SiteLocalizationOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<SecurityHeadersOptions>()
            .Bind(configuration.GetSection(SecurityHeadersOptions.SectionName))
            .ValidateOnStart();

        services
            .AddOptions<AnalyticsOptions>()
            .Bind(configuration.GetSection(AnalyticsOptions.SectionName))
            .ValidateOnStart();

        services
            .AddOptions<ImageDeliveryOptions>()
            .Bind(configuration.GetSection(ImageDeliveryOptions.SectionName))
            .ValidateOnStart();

        services
            .AddOptions<SiteOperationsOptions>()
            .Bind(configuration.GetSection(SiteOperationsOptions.SectionName))
            .ValidateOnStart();

        services
            .AddOptions<EmailDeliveryOptions>()
            .Bind(configuration.GetSection(EmailDeliveryOptions.SectionName))
            .ValidateOnStart();

        services
            .AddOptions<ObservabilityOptions>()
            .Bind(configuration.GetSection(ObservabilityOptions.SectionName))
            .ValidateOnStart();

        services
            .AddOptions<FeatureFlagsOptions>()
            .Bind(configuration.GetSection(FeatureFlagsOptions.SectionName))
            .ValidateOnStart();

        services
            .AddOptions<FutureDataConnectionsOptions>()
            .Bind(configuration.GetSection(FutureDataConnectionsOptions.SectionName))
            .ValidateOnStart();

        services
            .AddOptions<FileHandlingOptions>()
            .Bind(configuration.GetSection(FileHandlingOptions.SectionName))
            .ValidateOnStart();

        var localizationConfig = configuration.GetSection(SiteLocalizationOptions.SectionName).Get<SiteLocalizationOptions>() ?? new SiteLocalizationOptions();
        var supportedCultures = localizationConfig.SupportedCultures
            .Select(culture => new CultureInfo(culture))
            .ToArray();

        services.AddLocalization(options => options.ResourcesPath = "Resources");

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(localizationConfig.DefaultCulture);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            options.RequestCultureProviders =
            [
                new RouteDataRequestCultureProvider
                {
                    RouteDataStringKey = "culture",
                    UIRouteDataStringKey = "culture"
                },
                new QueryStringRequestCultureProvider(),
                new CookieRequestCultureProvider(),
                new AcceptLanguageHeaderRequestCultureProvider()
            ];
        });

        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Strict;
        });

        services.AddOptions<AntiforgeryOptions>()
            .Configure<IWebHostEnvironment>((options, environment) =>
            {
                options.Cookie.SecurePolicy = environment.IsDevelopment()
                    ? CookieSecurePolicy.SameAsRequest
                    : CookieSecurePolicy.Always;
            });

        services.AddOptions<CookiePolicyOptions>()
            .Configure<IWebHostEnvironment>((options, environment) =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
                options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
                options.Secure = environment.IsDevelopment()
                    ? CookieSecurePolicy.SameAsRequest
                    : CookieSecurePolicy.Always;
            });

        services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(kvp => kvp.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

                return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(new
                {
                    message = "Validation failed.",
                    statusCode = StatusCodes.Status400BadRequest,
                    traceId = context.HttpContext.TraceIdentifier,
                    errors
                });
            };
        });

        services.AddScoped<IWebsiteContentService, WebsiteContentService>();
        services.AddScoped<ICompanyProfileService, CompanyProfileService>();
        services.AddScoped<IProductCatalogService, ProductCatalogService>();
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IPartnershipApplicationService, PartnershipApplicationService>();
        services.AddScoped<ISeoService, SeoService>();
        services.AddSingleton<IImageUrlService, ImageUrlService>();
        services.AddScoped<IAiAssistantService, AiAssistantService>();
        services.AddSingleton<IKnowledgeLoader, KnowledgeLoader>();
        services.AddSingleton<IKnowledgeValidator, KnowledgeValidator>();
        services.AddSingleton<IKnowledgeRepository, KnowledgeRepository>();
        services.AddSingleton<IKnowledgeBaseService, KnowledgeBaseService>();
        services.AddSingleton<IKnowledgeSearchService, KnowledgeSearchService>();
        services.AddSingleton<IKnowledgeService, KnowledgeService>();
        services.AddScoped<IAIContextBuilder, AIContextBuilder>();
        services.AddScoped<IAIResponseBuilder, AIResponseBuilder>();
        services.AddSingleton<IAiKnowledgeBaseService, AiKnowledgeBaseService>();
        services.AddScoped<IAiProviderFactory, AiProviderFactory>();
        services.AddScoped<IAiProvider, RuleBasedAiProvider>();
        services.AddScoped<IAiProvider, OpenAiProvider>();
        services.AddSingleton<IEnquiryStore, InMemoryEnquiryStore>();
        services.AddScoped<IEmailNotificationService, SmtpPlaceholderEmailNotificationService>();
        services.AddSingleton<IFileSecurityValidator, FileSecurityValidator>();
        services.AddTransient<SecurityHeadersMiddleware>();
        services.AddTransient<MaintenanceModeMiddleware>();
        services.AddTransient<GlobalExceptionMiddleware>();
        services.AddTransient<RequestTelemetryMiddleware>();
        services.AddHostedService<ConfigurationAuditHostedService>();

        var aiRateLimit = configuration.GetSection(AiAssistantOptions.SectionName)
            .Get<AiAssistantOptions>()?.RateLimit ?? new RateLimitOptions();

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy("AiAssistantPolicy", _ =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: "ai-assistant",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = aiRateLimit.PermitLimit,
                        Window = TimeSpan.FromSeconds(aiRateLimit.WindowSeconds),
                        QueueLimit = aiRateLimit.QueueLimit,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        AutoReplenishment = true
                    }));

            options.AddPolicy("ContactPolicy", _ =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: "contact-enquiry",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(10),
                        QueueLimit = 0,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        AutoReplenishment = true
                    }));
        });

        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        services.AddResponseCaching();
        services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live", "ready"]);

        return services;
    }
}
