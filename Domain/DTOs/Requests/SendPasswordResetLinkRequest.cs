using System.ComponentModel.DataAnnotations;

namespace DTOs.Requests;

public sealed class SendPasswordResetLinkRequest
{
    [Display(Name = "Почта")]
    public string Email { get; set; } = string.Empty;
}
