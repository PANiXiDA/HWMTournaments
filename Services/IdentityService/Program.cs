using DAL.DependencyInjection;

using IdentityService.Extensions;
using IdentityService.Extensions.Configurations;

using RedisClient;

var builder = WebApplication.CreateBuilder(args);

#region Custom Extensions

builder.ConfigureKestrelPorts();

builder.Services.UseDAL(builder.Configuration);

builder.Services.ConfigureGrpcServices();
builder.Services.ConfigureIdentityServer(builder.Configuration);
builder.Services.UseRedis(builder.Configuration);
builder.Services.UseAutoMapper();

#endregion

var app = builder.Build();

app.MapGrpcEndpoints();
app.UseIdentityServer();
await app.Services.SynchronizerEnumRolesAsync();

app.Run();
