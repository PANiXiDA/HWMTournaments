using Microsoft.AspNetCore.Identity;

namespace DAL.DbModels.Identity;

public sealed class ApplicationUser : IdentityUser<int>
{
    public User? User { get; set; }

    public ICollection<ApplicationUserRoleScope> Roles { get; set; } = [];
}
