using DAL.DependencyInjection;
using DAL.EF;
using DAL.Migrator.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((ctx, cfg) =>
    {
        cfg.AddJsonFile("appsettings.json")
       .AddEnvironmentVariables();
    })
    .ConfigureServices((ctx, services) =>
    {
        services.UseDAL(ctx.Configuration);
    })
    .RunMigrationsAsync<DefaultDbContext>();
