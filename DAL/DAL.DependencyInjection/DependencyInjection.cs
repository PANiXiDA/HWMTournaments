using DAL.EF;
using DAL.Implementations;
using DAL.Interfaces;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DAL.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection UseDAL(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<DefaultDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnectionString")).UseSnakeCaseNamingConvention());
        serviceCollection.AddDataProtection().PersistKeysToDbContext<DefaultDbContext>();
        serviceCollection.AddScoped<ISettingsDAL, SettingsDAL>();
        serviceCollection.AddScoped<IUsersDAL, UsersDAL>();

        return serviceCollection;
    }
}