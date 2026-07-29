namespace MamiaSeedsOil.Web.Configuration;

public sealed class EmailDeliveryOptions
{
    public const string SectionName = "EmailDelivery";

    public string Provider { get; set; } = "[To Be Updated]";
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string RecipientEmail { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;

    public SmtpProviderOptions Smtp { get; set; } = new();
    public Microsoft365ProviderOptions Microsoft365 { get; set; } = new();
    public ApiProviderOptions SendGrid { get; set; } = new();
    public ApiProviderOptions Mailgun { get; set; } = new();
    public ApiProviderOptions Resend { get; set; } = new();
}

public sealed class SmtpProviderOptions
{
    public bool Enabled { get; set; }
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool UseStartTls { get; set; } = true;
}

public sealed class Microsoft365ProviderOptions
{
    public bool Enabled { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string SenderUpn { get; set; } = string.Empty;
}

public sealed class ApiProviderOptions
{
    public bool Enabled { get; set; }
    public string ApiKey { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
}
