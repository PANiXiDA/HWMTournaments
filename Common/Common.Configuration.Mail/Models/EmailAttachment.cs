namespace Common.Configuration.Mail.Models;

public sealed record EmailAttachment(
    string FileName,
    Stream Content,
    string? ContentType = null);