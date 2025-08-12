using System.ComponentModel.DataAnnotations;

namespace DTOs.Requests;

public sealed class SendEmailConfirmationLinkRequest
{
    [Display(Name = "Почта")]
    public string Email { get; set; } = string.Empty;
}
