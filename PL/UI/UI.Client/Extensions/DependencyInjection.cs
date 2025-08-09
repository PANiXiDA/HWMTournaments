using UI.Client.Services;

namespace UI.Client.Extensions;

public static class DependencyInjection
{
    public static void UseServices(this IServiceCollection services)
    {
        services.AddScoped<NotificationService>();
        services.AddScoped<TokenService>();
        services.AddScoped<AuthService>();
    }
}
