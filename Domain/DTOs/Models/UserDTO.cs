using Common.Enums;

using DTOs.Models.Core;

namespace DTOs.Models;

public sealed class UserDTO : BaseDTO<int>
{
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public bool PhoneNumberConfirmed { get; set; }

    public List<ApplicationUserRole> Roles { get; set; } = [];
}
