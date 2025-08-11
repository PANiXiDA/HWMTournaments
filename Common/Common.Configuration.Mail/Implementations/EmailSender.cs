using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;

using Common.Configuration.Mail.Interfaces;
using Common.Configuration.Mail.Models;

using MailKit.Net.Smtp;
using MailKit.Security;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MimeKit;

using ContentDisposition = MimeKit.ContentDisposition;
using ContentType = MimeKit.ContentType;

namespace Common.Configuration.Mail.Implementations;

public sealed class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;
    private readonly SmtpConfiguration _smtpConfiguration;

    public EmailSender(
        ILogger<EmailSender> logger,
        IOptions<SmtpConfiguration> options)
    {
        _logger = logger;
        _smtpConfiguration = options.Value;
    }

    public async Task SendAsync(
        IEnumerable<string> recipients,
        string subject,
        string htmlBody,
        string? textBody = null,
        string? senderName = null,
        string? replyTo = null,
        IEnumerable<EmailAttachment>? attachments = null,
        bool sendBlindCopies = true,
        CancellationToken cancellationToken = default)
    {
        var toList = recipients?.Where(recipient => !string.IsNullOrWhiteSpace(recipient)).Distinct().ToList() ?? throw new ArgumentNullException(nameof(recipients));
        if (toList.Count == 0)
        {
            throw new ArgumentException("Список получателей пуст.", nameof(recipients));
        }

        var finalHtml = await WrapWithTemplateAsync(subject, htmlBody, textBody, cancellationToken).ConfigureAwait(false);

        using var client = new SmtpClient
        {
            Timeout = Math.Max(1, _smtpConfiguration.TimeoutSeconds) * 1000
        };

        try
        {
            var secure = ParseSecureSocket(_smtpConfiguration.SecureSocket);
            await client.ConnectAsync(_smtpConfiguration.Host, _smtpConfiguration.Port, secure, cancellationToken).ConfigureAwait(false);

            client.AuthenticationMechanisms.Remove("XOAUTH2");

            if (!string.IsNullOrEmpty(_smtpConfiguration.UserName))
            {
                await client.AuthenticateAsync(_smtpConfiguration.UserName, _smtpConfiguration.UserPassword, cancellationToken).ConfigureAwait(false);
            }

            var message = BuildMessage(toList, subject, finalHtml, textBody, senderName, replyTo, attachments, sendBlindCopies);

            _logger.LogInformation("Отправка письма на {Count} адресов, тема: {Subject}", toList.Count, subject);

            await client.SendAsync(message, cancellationToken).ConfigureAwait(false);
        }
        catch (SmtpCommandException ex)
        {
            _logger.LogError(ex, "SMTP ошибка при отправке письма: {StatusCode} {Response}", ex.StatusCode, ex.Message);
            throw;
        }
        catch (SmtpProtocolException ex)
        {
            _logger.LogError(ex, "SMTP протокольная ошибка при отправке письма");
            throw;
        }
        finally
        {
            try
            {
                if (client.IsConnected)
                {
                    await client.DisconnectAsync(true, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Ошибка при закрытии SMTP соединения");
            }
        }
    }

    private MimeMessage BuildMessage(
        List<string> recipients,
        string subject,
        string htmlBody,
        string? textBody,
        string? senderName,
        string? replyTo,
        IEnumerable<EmailAttachment>? attachments,
        bool sendBlindCopies)
    {
        var fromAddress = _smtpConfiguration.FromAddress?.Trim();
        if (string.IsNullOrEmpty(fromAddress))
        {
            fromAddress = _smtpConfiguration.UserName;
        }

        var message = new MimeMessage
        {
            Subject = subject
        };

        message.From.Add(new MailboxAddress(
            name: string.IsNullOrWhiteSpace(senderName) ? _smtpConfiguration.FromName : senderName,
            address: fromAddress));

        if (!string.IsNullOrWhiteSpace(replyTo))
        {
            message.ReplyTo.Add(MailboxAddress.Parse(replyTo));
        }

        var toFirst = recipients[0];
        if (sendBlindCopies && recipients.Count > 1)
        {
            message.To.Add(MailboxAddress.Parse(toFirst));
            for (int i = 1; i < recipients.Count; i++)
            {
                message.Bcc.Add(MailboxAddress.Parse(recipients[i]));
            }
        }
        else
        {
            foreach (var recipient in recipients)
            {
                message.To.Add(MailboxAddress.Parse(recipient));
            }
        }

        var builder = new BodyBuilder
        {
            HtmlBody = htmlBody,
            TextBody = textBody ?? HtmlToPlainText(htmlBody)
        };

        if (attachments != null)
        {
            foreach (var attachment in attachments)
            {
                var contentType = attachment.ContentType ?? MediaTypeNames.Application.Octet;
                builder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(contentType));
            }
        }

        if (htmlBody?.IndexOf("cid:app-favicon", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            var iconPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "app-icon.png");
            if (File.Exists(iconPath))
            {
                var imgPart = (MimePart)builder.LinkedResources.Add(
                    "app-icon.png",
                    File.ReadAllBytes(iconPath),
                    ContentType.Parse("image/png"));

                imgPart.ContentId = "app-favicon";
                imgPart.ContentDisposition = new ContentDisposition(ContentDisposition.Inline);
                imgPart.ContentTransferEncoding = ContentEncoding.Base64;
            }
        }

        message.Body = builder.ToMessageBody();

        message.Headers.Add("X-Priority", "3 (Normal)");

        return message;
    }

    private async Task<string> WrapWithTemplateAsync(string subject, string innerHtml, string? textBody, CancellationToken ct)
    {
        var template = await LoadTemplateAsync(ct).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(template))
        {
            _logger.LogWarning("Email template not found, sending raw htmlBody.");
            return innerHtml;
        }

        var preheaderSource = textBody ?? HtmlToPlainText(innerHtml);
        var preheader = Truncate(preheaderSource, 140);

        var footer = $"© {DateTime.UtcNow:yyyy} {_smtpConfiguration.FromName}. Все права защищены.";

        var html = template;
        html = html.Replace("{{Subject}}", subject, StringComparison.Ordinal);
        html = html.Replace("{{BrandName}}", _smtpConfiguration.FromName, StringComparison.Ordinal);
        html = html.Replace("{{Title}}", subject, StringComparison.Ordinal);
        html = html.Replace("{{Preheader}}", preheader, StringComparison.Ordinal);
        html = html.Replace("{{MessageHtml}}", innerHtml, StringComparison.Ordinal);
        html = html.Replace("{{FooterHtml}}", footer, StringComparison.Ordinal);

        html = Regex.Replace(html, "{{[^}]+}}", string.Empty);

        return html;
    }

    private static async Task<string?> LoadTemplateAsync(CancellationToken ct)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Templates", "Template.html");
        if (File.Exists(path))
        {
            return await File.ReadAllTextAsync(path, Encoding.UTF8, ct).ConfigureAwait(false);
        }

        return null;
    }

    private static string HtmlToPlainText(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }
        var text = Regex.Replace(html, "<.*?>", " ");

        return WebUtility.HtmlDecode(Regex.Replace(text, @"\s+", " ").Trim());
    }

    private static string Truncate(string s, int max)
    {
        return string.IsNullOrEmpty(s) ? string.Empty : (s.Length <= max ? s : s[..max]).Trim();
    }

    private static SecureSocketOptions ParseSecureSocket(string value) =>
        value?.Trim().ToLowerInvariant() switch
        {
            "none" => SecureSocketOptions.None,
            "ssl" or "sslonconnect" => SecureSocketOptions.SslOnConnect,
            "starttls" => SecureSocketOptions.StartTls,
            "auto" => SecureSocketOptions.Auto,
            _ => SecureSocketOptions.StartTlsWhenAvailable
        };
}
