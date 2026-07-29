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

    private async Task SendMailAsync(string subject, string body, string senderEmail, CancellationToken cancellationToken)
    {
        var smtpOptions = _options.Smtp;
        using var message = new MailMessage
        {
            From = new MailAddress(GetFromEmail(), GetFromName()),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
            BodyEncoding = Encoding.UTF8,
            SubjectEncoding = Encoding.UTF8
        };

        message.To.Add(new MailAddress(GetRecipientEmail()));
        if (!string.IsNullOrWhiteSpace(senderEmail))
        {
            message.ReplyToList.Add(new MailAddress(senderEmail));
        }

        using var client = new SmtpClient(smtpOptions.Host, smtpOptions.Port)
        {
            EnableSsl = smtpOptions.UseStartTls,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Timeout = 15000
        };

        if (!string.IsNullOrWhiteSpace(smtpOptions.Username))
        {
            client.Credentials = new NetworkCredential(smtpOptions.Username, smtpOptions.Password);
        }

        await client.SendMailAsync(message);
        cancellationToken.ThrowIfCancellationRequested();
    }

    private bool IsConfigured()
    {
        var smtpOptions = _options.Smtp;
        return smtpOptions.Enabled
            && !string.IsNullOrWhiteSpace(smtpOptions.Host)
            && !string.IsNullOrWhiteSpace(GetRecipientEmail())
            && !string.IsNullOrWhiteSpace(GetFromEmail());
    }

    private string GetRecipientEmail() => !string.IsNullOrWhiteSpace(_options.RecipientEmail)
        ? _options.RecipientEmail
        : !string.IsNullOrWhiteSpace(_options.FromEmail)
            ? _options.FromEmail
            : "info@mamiaseedsoil.com";

    private string GetFromEmail() => !string.IsNullOrWhiteSpace(_options.FromEmail)
        ? _options.FromEmail
        : "noreply@mamiaseedsoil.com";

    private string GetFromName() => !string.IsNullOrWhiteSpace(_options.FromName)
        ? _options.FromName
        : "Mamia Seeds Oil Limited";

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
