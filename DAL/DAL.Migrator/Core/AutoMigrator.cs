using DAL.Migrator.Extensions;

using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Npgsql.EntityFrameworkCore.PostgreSQL.Design.Internal;

namespace DAL.Migrator.Core;

internal static class AutoMigrator
{
    internal static async Task<IHost> RunMigrationsAsync<TContext>(this IHostBuilder builder) where TContext : DbContext
    {
        using var host = builder.Build();
        using var scope = host.Services.CreateScope();

        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var projectPath = configuration["EF:ProjectPath"] ?? throw new InvalidOperationException("Не задан EF:ProjectPath");
        var projectPathAbsolute = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, projectPath));

        var applyEntities = configuration.GetValue("ApplyEntities", true);
        var applyHistory = configuration.GetValue("ApplyHistory", true);

        var db = scope.ServiceProvider.GetRequiredService<TContext>();

        var services = new ServiceCollection()
            .AddEntityFrameworkDesignTimeServices()
            .AddDbContextDesignTimeServices(db);

#pragma warning disable EF1001 // Internal EF Core API usage
        new NpgsqlDesignTimeServices().ConfigureDesignTimeServices(services);
#pragma warning restore EF1001 // Internal EF Core API usage

        var designServiceProvider = services.BuildServiceProvider();

        var difference = MigrationsDifferenceProvider.GetDifferences(db, designServiceProvider);
        if (!difference.Any())
        {
            await db.Database.MigrateAsync();
            return host;
        }

        var scaffoldedMigration = MigrationsCreator.CreateAndSaveMigration(
            designServiceProvider: designServiceProvider,
            difference: difference,
            contextNamespace: typeof(TContext).Namespace!,
            projectPath: projectPathAbsolute
        );

        await MigrationsApplier.ApplyMigrationAsync(
            db: db,
            difference: difference,
            migrationId: scaffoldedMigration.MigrationId,
            applyEntities: applyEntities,
            applyHistory: applyHistory
        );

        return host;
    }
}
