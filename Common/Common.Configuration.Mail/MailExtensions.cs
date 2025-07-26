using MailKit.Net.Smtp;

using MimeKit;
using MimeKit.Text;

namespace Common.Configuration.Mail;

public static class MailExtensions
{
    public static async Task SendAsync(
        this SmtpClient client,
        SmtpConfiguration configuration,
        string senderName,
        IEnumerable<string> recipients,
        string subject,
        string body,
        bool sendBlindCopies = true)
    {
        await client.ConnectAsync(configuration.Host, configuration.Port, configuration.UseSsl);
        await client.AuthenticateAsync(configuration.UserName, configuration.UserPassword);

        var message = new MimeMessage
        {
            Subject = subject,
            Body = new TextPart(TextFormat.Html)
            {
                Text = body
            }
        };

        message.From.Add(new MailboxAddress(senderName, configuration.UserName));
        recipients = recipients.ToList();

        if (sendBlindCopies && recipients.Count() > 1)
        {
            message.To.Add(new MailboxAddress(null, recipients.First()));
            message.Bcc.AddRange(recipients.Select(item => new MailboxAddress(null, item)).Skip(1));
        }
        else
        {
            message.To.AddRange(recipients.Select(item => new MailboxAddress(null, item)));
        }

        await client.SendAsync(message);
    }
}
