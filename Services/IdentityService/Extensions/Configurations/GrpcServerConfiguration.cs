using IdentityService.Middlewares;
using IdentityService.Services;

namespace IdentityService.Extensions.Configurations;

public static class GrpcServerConfiguration
{
    public static void ConfigureGrpcServices(this IServiceCollection services)
    {
        services.AddSingleton<ExceptionInterceptor>();
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<ExceptionInterceptor>();
        });
        services.AddGrpcReflection();
    }

    public static void MapGrpcEndpoints(this WebApplication app)
    {
        app.MapGrpcService<ApplicationUserServiceImpl>();

        if (app.Environment.IsDevelopment())
        {
            app.MapGrpcReflectionService();
        }
    }
}
