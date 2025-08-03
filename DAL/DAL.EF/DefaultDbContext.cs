using DAL.DbModels;
using DAL.DbModels.Identity;
using DAL.EF.Configurations;

using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.EF;

public class DefaultDbContext : IdentityDbContext<
        ApplicationUser,
        IdentityRole<int>,
        int,
        IdentityUserClaim<int>,
        ApplicationUserRoleScope,
        IdentityUserLogin<int>,
        IdentityRoleClaim<int>,
        IdentityUserToken<int>>, IDataProtectionKeyContext
{
    public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        IdentityConfiguration.Configure(builder);
    }

    #region IdentityService

    public virtual DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public virtual DbSet<ApplicationUserRoleScope> ApplicationUserRoles { get; set; }

    #endregion

    public virtual DbSet<Settings> Settings { get; set; }
    public virtual DbSet<User> DomainUsers { get; set; }
    public virtual DbSet<Tournament> Tournaments { get; set; }
}