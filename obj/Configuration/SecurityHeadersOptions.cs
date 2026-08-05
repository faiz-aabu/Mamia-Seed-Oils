namespace MamiaSeedsOil.Web.Configuration;

public sealed class SecurityHeadersOptions
{
    public const string SectionName = "SecurityHeaders";

    public bool Enabled { get; set; } = true;
    public bool EnableCsp { get; set; } = true;
    public string ContentSecurityPolicy { get; set; } = "default-src 'self'; img-src 'self' data: https:; style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; font-src 'self' https://fonts.gstatic.com; script-src 'self' 'unsafe-inline' https://www.googletagmanager.com https://www.google-analytics.com https://www.clarity.ms https://connect.facebook.net; connect-src 'self' https://www.google-analytics.com https://region1.google-analytics.com https://www.clarity.ms; frame-ancestors 'none'; base-uri 'self'; form-action 'self'";
    public bool EnableHstsHeader { get; set; } = true;
    public bool EnableXContentTypeOptions { get; set; } = true;
    public bool EnableFrameDeny { get; set; } = true;
    public bool EnableReferrerPolicy { get; set; } = true;
    public string ReferrerPolicy { get; set; } = "strict-origin-when-cross-origin";
    public bool EnablePermissionsPolicy { get; set; } = true;
    public string PermissionsPolicy { get; set; } = "geolocation=(), microphone=(), camera=(), payment=(), usb=(), accelerometer=()";
    public bool EnableCrossOriginPolicies { get; set; } = true;
    public string CrossOriginOpenerPolicy { get; set; } = "same-origin";
    public string CrossOriginResourcePolicy { get; set; } = "same-origin";
    public string CrossOriginEmbedderPolicy { get; set; } = "credentialless";
}
