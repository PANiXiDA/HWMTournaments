using System.ComponentModel.DataAnnotations;

namespace DTOs.Requests;

public sealed class RegistrationRequest
{
    [Display(Name = "Имя")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Почта")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;
}
