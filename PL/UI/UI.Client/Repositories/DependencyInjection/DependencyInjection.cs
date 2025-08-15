using UI.Client.Repositories.Implementations;
using UI.Client.Repositories.Interfaces;

namespace UI.Client.Repositories.DependencyInjection;

public static class DependencyInjection
{
    public static void UseRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IAccessTokenRepository, AccessTokenRepository>();
    }
}
