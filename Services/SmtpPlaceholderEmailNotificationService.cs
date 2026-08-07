using System.Net;
using System.Net.Mail;
using System.Text;
using MamiaSeedsOil.Web.Configuration;
using MamiaSeedsOil.Web.Helpers;
using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Models;
using Microsoft.Extensions.Options;

namespace MamiaSeedsOil.Web.Services;

public sealed class SmtpPlaceholderEmailNotificationService : IEmailNotificationService
{
    private readonly ILogger<SmtpPlaceholderEmailNotificationService> _logger;
    private readonly EmailDeliveryOptions _options;

    public SmtpPlaceholderEmailNotificationService(ILogger<SmtpPlaceholderEmailNotificationService> logger, IOptions<EmailDeliveryOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task SendContactEnquiryNotificationAsync(ContactEnquiry enquiry, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!IsConfigured())
        {
            _logger.LogWarning("SMTP delivery is not configured for contact enquiry. Skipping send for {EmailMasked}", PrivacyMasker.MaskEmail(enquiry.Email));
            return;
        }

        var subject = $"New contact enquiry from {WebUtility.HtmlEncode(enquiry.FullName)}";
        var body = BuildHtmlBody(
            "New Contact Enquiry",
            [
                ("Full Name", enquiry.FullName),
                ("Company", enquiry.CompanyName),
                ("Email", enquiry.Email),
                ("Phone", enquiry.Phone),
                ("Message", enquiry.Message)
            ]);

        await SendMailAsync(subject, body, enquiry.Email, cancellationToken);
        _logger.LogInformation("Sent contact enquiry notification to {Recipient} for {EmailMasked}", GetRecipientEmail(), PrivacyMasker.MaskEmail(enquiry.Email));
    }

    public async Task SendDistributorEnquiryNotificationAsync(DistributorEnquiry enquiry, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!IsConfigured())
        {
            _logger.LogWarning("SMTP delivery is not configured for distributor enquiry. Skipping send for {EmailMasked}", PrivacyMasker.MaskEmail(enquiry.Email));
            return;
        }

        var subject = $"New distributor enquiry from {WebUtility.HtmlEncode(enquiry.CompanyName)}";
        var body = BuildHtmlBody(
            "New Distributor Enquiry",
            [
                ("Company", enquiry.CompanyName),
                ("Contact Person", enquiry.ContactPerson),
                ("Email", enquiry.Email),
                ("Phone", enquiry.Phone),
                ("Country", enquiry.Country),
                ("State", enquiry.State),
                ("Business Type", enquiry.BusinessType),
                ("Expected Monthly Volume", enquiry.ExpectedMonthlyVolume),
                ("Message", enquiry.Message)
            ]);

        await SendMailAsync(subject, body, enquiry.Email, cancellationToken);
        _logger.LogInformation("Sent distributor enquiry notification to {Recipient} for {EmailMasked}", GetRecipientEmail(), PrivacyMasker.MaskEmail(enquiry.Email));
    }

    public async Task SendPartnershipApplicationNotificationAsync(PartnershipApplication application, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!IsConfigured())
        {
            _logger.LogWarning("SMTP delivery is not configured for partnership application. Skipping send for {EmailMasked}", PrivacyMasker.MaskEmail(application.EmailAddress));
            return;
        }

        var subject = $"New partnership application from {WebUtility.HtmlEncode(application.CompanyName)}";
        var body = BuildHtmlBody(
            "New Partnership Application",
            [
                ("Company", application.CompanyName),
                ("Contact Person", application.ContactPerson),
                ("Email", application.EmailAddress),
                ("Phone", application.PhoneNumber),
                ("Additional Notes", application.AdditionalNotes)
            ]);

        await SendMailAsync(subject, body, application.EmailAddress, cancellationToken);
        _logger.LogInformation("Sent partnership application notification to {Recipient} for {EmailMasked}", GetRecipientEmail(), PrivacyMasker.MaskEmail(application.EmailAddress));
    }

    public async Task SendDistributorApplicationNotificationAsync(DistributorApplication application, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var recipientEmail = ResolveRecipientEmail();
        var senderEmail = ResolveFromEmail();
        var provider = ResolveProvider();

        _logger.LogInformation("✔ Distributor email service started. Provider={Provider}; Recipient={Recipient}; Sender={Sender}", provider, recipientEmail, senderEmail);

        if (!IsConfigured())
        {
            _logger.LogWarning("Email delivery configuration is incomplete for distributor application. Provider={Provider}; Recipient={Recipient}; Sender={Sender}; Host={Host}; Port={Port}; Username={Username}; PasswordConfigured={PasswordConfigured}",
                provider,
                recipientEmail,
                senderEmail,
                ResolveSmtpHost(),
                ResolveSmtpPort(),
                ResolveSmtpUsername(),
                !string.IsNullOrWhiteSpace(ResolveSmtpPassword()));
            return;
        }

        var subject = $"New Distributor Application - {WebUtility.HtmlEncode(application.BusinessName)}";
        var fields = new List<(string Label, string? Value)>
        {
            ("Full Name", application.FullName),
            ("Business Name", application.BusinessName),
            ("Email Address", application.EmailAddress),
            ("Phone Number", application.PhoneNumber),
            ("WhatsApp Number", application.WhatsAppNumber),
            ("State", application.State),
            ("City", application.City),
            ("Business Address", application.BusinessAddress),
            ("Country", application.Country),
            ("Business Type", application.BusinessType),
            ("Expected Monthly Order Quantity", application.ExpectedMonthlyOrderQuantity),
            ("Preferred Products", string.Join(", ", application.PreferredProducts)),
            ("Warehouse Available", application.WarehouseAvailable.HasValue ? (application.WarehouseAvailable.Value ? "Yes" : "No") : string.Empty),
            ("Can Handle Bulk Orders", application.CanHandleBulkOrders.HasValue ? (application.CanHandleBulkOrders.Value ? "Yes" : "No") : string.Empty),
            ("Areas You Can Supply", application.AreasYouCanSupply),
            ("Agreement", application.AgreedToTerms ? "Yes" : "No")
        };

        if (string.Equals(application.BusinessType, "Distributor", StringComparison.OrdinalIgnoreCase))
        {
            fields.Add(("Vehicle Type", application.VehicleType));
            fields.Add(("Number of Vehicles", application.NumberOfVehicles));
        }

        var body = BuildHtmlBody("New Distributor Application", fields);

        try
        {
            _logger.LogInformation("✔ SMTP connection started. Host={Host}; Port={Port}; EnableSsl={EnableSsl}", ResolveSmtpHost(), ResolveSmtpPort(), ResolveUseStartTls());
            await SendMailAsync(subject, body, application.EmailAddress, cancellationToken);
            _logger.LogInformation("✔ Email sent successfully to {Recipient} for {EmailMasked}", recipientEmail, PrivacyMasker.MaskEmail(application.EmailAddress));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "✘ Email sending failed for distributor application. Recipient={Recipient}; Sender={Sender}", recipientEmail, senderEmail);
            throw;
        }
    }

    private async Task SendMailAsync(string subject, string body, string senderEmail, CancellationToken cancellationToken)
    {
        var smtpHost = ResolveSmtpHost();
        var smtpPort = ResolveSmtpPort();
        var smtpUsername = ResolveSmtpUsername();
        var smtpPassword = ResolveSmtpPassword();
        var enableSsl = ResolveUseStartTls();
        using var message = new MailMessage
        {
            From = new MailAddress(ResolveFromEmail(), ResolveFromName()),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
            BodyEncoding = Encoding.UTF8,
            SubjectEncoding = Encoding.UTF8
        };

        message.To.Add(new MailAddress(ResolveRecipientEmail()));
        if (!string.IsNullOrWhiteSpace(senderEmail))
        {
            message.ReplyToList.Add(new MailAddress(senderEmail));
        }

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            EnableSsl = enableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Timeout = 15000
        };

        if (!string.IsNullOrWhiteSpace(smtpUsername))
        {
            client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
        }

        await client.SendMailAsync(message);
        cancellationToken.ThrowIfCancellationRequested();
    }

    private bool IsConfigured()
    {
        var smtpEnabled = _options.Smtp.Enabled || IsEnvironmentEnabled("EmailDelivery__Smtp__Enabled");
        return smtpEnabled
            && !string.IsNullOrWhiteSpace(ResolveProvider())
            && !string.IsNullOrWhiteSpace(ResolveSmtpHost())
            && ResolveSmtpPort() > 0
            && !string.IsNullOrWhiteSpace(ResolveSmtpUsername())
            && !string.IsNullOrWhiteSpace(ResolveSmtpPassword())
            && !string.IsNullOrWhiteSpace(ResolveRecipientEmail())
            && !string.IsNullOrWhiteSpace(ResolveFromEmail());
    }

    private string ResolveProvider() => GetSetting("EmailDelivery__Provider", _options.Provider, "Smtp");

    private string ResolveRecipientEmail() => GetSetting("EmailDelivery__RecipientEmail", _options.RecipientEmail, "info@mamiaseedsoil.com");

    private string GetRecipientEmail() => ResolveRecipientEmail();

    private string ResolveFromEmail() => GetSetting("EmailDelivery__FromEmail", _options.FromEmail, "noreply@mamiaseedsoil.com");

    private string ResolveFromName() => GetSetting("EmailDelivery__FromName", _options.FromName, "Mamia Seeds Oil Limited");

    private string ResolveSmtpHost() => GetSetting("EmailDelivery__Smtp__Host", _options.Smtp.Host, string.Empty);

    private int ResolveSmtpPort() => int.TryParse(GetSetting("EmailDelivery__Smtp__Port", _options.Smtp.Port.ToString(), "587"), out var port) ? port : 587;

    private string ResolveSmtpUsername() => GetSetting("EmailDelivery__Smtp__Username", _options.Smtp.Username, string.Empty);

    private string ResolveSmtpPassword() => GetSetting("EmailDelivery__Smtp__Password", _options.Smtp.Password, string.Empty);

    private bool ResolveUseStartTls() => bool.TryParse(GetSetting("EmailDelivery__Smtp__UseStartTls", _options.Smtp.UseStartTls.ToString(), "true"), out var useTls) && useTls;

    private static bool IsEnvironmentEnabled(string key)
    {
        return bool.TryParse(Environment.GetEnvironmentVariable(key), out var enabled) && enabled;
    }

    private static string GetSetting(string key, string optionValue, string defaultValue)
    {
        var fromEnvironment = Environment.GetEnvironmentVariable(key);
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
        {
            return fromEnvironment;
        }

        return string.IsNullOrWhiteSpace(optionValue) ? defaultValue : optionValue;
    }

    private static string BuildHtmlBody(string title, IEnumerable<(string Label, string? Value)> fields)
    {
        var rows = string.Join(Environment.NewLine, fields.Select(field =>
            $"<tr><td style=\"padding: 8px 0; font-weight: 600; color: #0f172a;\">{WebUtility.HtmlEncode(field.Label)}</td><td style=\"padding: 8px 0; color: #334155;\">{WebUtility.HtmlEncode(field.Value ?? string.Empty)}</td></tr>"));

        return $"""
            <html>
            <body style="font-family: Arial, sans-serif; background-color: #f8fafc; padding: 24px; color: #0f172a;">
                <div style="max-width: 640px; margin: 0 auto; background: #ffffff; border: 1px solid #e2e8f0; border-radius: 12px; padding: 24px;">
                    <h2 style="margin-top: 0; color: #0f172a;">{WebUtility.HtmlEncode(title)}</h2>
                    <p style="margin-bottom: 16px;">A new enquiry has been submitted through the website contact form.</p>
                    <table style="width: 100%; border-collapse: collapse;">
                        {rows}
                    </table>
                </div>
            </body>
            </html>
            """;
    }
}
