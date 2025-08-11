using Common.Configuration.Mail.Models;

namespace Common.Configuration.Mail.Interfaces;

public interface IEmailSender
{
    Task SendAsync(
        IEnumerable<string> recipients,
        string subject,
        string htmlBody,
        string? textBody = null,
        string? senderName = null,
        string? replyTo = null,
        IEnumerable<EmailAttachment>? attachments = null,
        bool sendBlindCopies = true,
        CancellationToken ct = default);
}
