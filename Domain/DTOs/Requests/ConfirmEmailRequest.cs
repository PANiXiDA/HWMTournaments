using System.ComponentModel.DataAnnotations;

namespace DTOs.Requests;

public sealed class ConfirmEmailRequest
{
    [Display(Name = "Почта")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Токен")]
    public string Token { get; set; } = string.Empty;
}
