using Microsoft.AspNetCore.Identity;

namespace DAL.DbModels.Identity;

public sealed class ApplicationUserRoleScope : IdentityUserRole<int>
{
    public ApplicationUser? ApplicationUser { get; set; }
    public IdentityRole<int>? Role { get; set; }
}
