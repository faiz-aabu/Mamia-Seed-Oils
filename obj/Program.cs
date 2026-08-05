using MamiaSeedsOil.Web.Extensions;
using MamiaSeedsOil.Web.Resources;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
});

builder.Services.AddHttpClient();

builder.Services
    .AddControllersWithViews(options =>
    {
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    })
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (_, factory) => factory.Create(typeof(SharedResource));
    });
builder.Services.AddWebConfiguration(builder.Configuration);

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");

app.Lifetime.ApplicationStarted.Register(() =>
{
    logger.LogInformation("Application startup complete. Environment={Environment}", app.Environment.EnvironmentName);
});

app.Lifetime.ApplicationStopping.Register(() =>
{
    logger.LogWarning("Application shutdown initiated.");
});

app.Lifetime.ApplicationStopped.Register(() =>
{
    logger.LogInformation("Application stopped cleanly.");
});

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();
app.UseWebPipeline();
app.MapWebRoutes();

app.Run();
 
