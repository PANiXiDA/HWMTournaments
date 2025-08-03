using Common.Constants;

using DAL.DbModels;
using DAL.DbModels.Identity;

using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAL.EF.Configurations;

public static class IdentityConfiguration
{
    public static void Configure(ModelBuilder builder)
    {
        builder.Entity<User>().ToTable("users");
        builder.Entity<ApplicationUserRoleScope>(e =>
        {
            e.ToTable("application_user_role_scopes", SchemeConstants.IdentityScheme);
            e.Property(r => r.UserId).HasColumnName("application_user_id");
            e.Property(r => r.RoleId).HasColumnName("role_id");
            e.HasOne(r => r.ApplicationUser).WithMany(u => u.Roles).HasForeignKey(r => r.UserId);
            e.HasOne(r => r.Role).WithMany().HasForeignKey(r => r.RoleId);
        });
        builder.Entity<ApplicationUser>().ToTable("application_users", SchemeConstants.IdentityScheme);
        builder.Entity<IdentityRole<int>>().ToTable("identity_roles", SchemeConstants.IdentityScheme);
        builder.Entity<IdentityUserClaim<int>>().ToTable("identity_user_claims", SchemeConstants.IdentityScheme);
        builder.Entity<IdentityUserLogin<int>>().ToTable("identity_user_logins", SchemeConstants.IdentityScheme);
        builder.Entity<IdentityRoleClaim<int>>().ToTable("identity_role_claims", SchemeConstants.IdentityScheme);
        builder.Entity<IdentityUserToken<int>>().ToTable("identity_user_tokens", SchemeConstants.IdentityScheme);
        builder.Entity<DataProtectionKey>().ToTable("data_protection_keys", SchemeConstants.IdentityScheme);
    }
}
