using System.ComponentModel.DataAnnotations;

namespace DTOs.Requests;

public sealed class EmailConfirmationRequest
{
    [Display(Name = "Логин")]
    public string Email { get; set; } = string.Empty;
}
