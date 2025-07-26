using DAL.DbModels;

using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.EF;

public class DefaultDbContext : DbContext, IDataProtectionKeyContext
{
    public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options)
    {
    }

    public virtual DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    public virtual DbSet<Settings> Settings { get; set; }
    public virtual DbSet<User> Users { get; set; }
}