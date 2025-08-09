using System.ComponentModel.DataAnnotations;

namespace DTOs.Requests;

public sealed class LoginRequest
{
    [Display(Name = "Логин")]
    public string Login { get; set; } = string.Empty;

    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;
}
