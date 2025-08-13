using UI.Client.Services.Implementations;
using UI.Client.Services.Interfaces;

namespace UI.Client.Services.DependencyInjection;

public static class DependencyInjection
{
    public static void UseServices(this IServiceCollection services)
    {
        services.AddScoped<NotificationService>();
        services.AddScoped<TokenService>();
        services.AddScoped<AuthService>();

        services.AddScoped<ITournamentsService, TournamentsService>();
        services.AddScoped<IUsersService, UsersService>();
    }
}
