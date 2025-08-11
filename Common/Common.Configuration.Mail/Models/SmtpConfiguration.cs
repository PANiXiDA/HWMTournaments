namespace Common.Configuration.Mail.Models;

public class SmtpConfiguration
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string SecureSocket { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserPassword { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; }
}
