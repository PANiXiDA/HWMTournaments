using Common.Constants;

using UI.Client.Clients.Implementations;
using UI.Client.Clients.Interfaces;

namespace UI.Client.Clients.DependencyInjection;

public static class DependencyInjection
{
    public static void UseHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient(ClientsConstants.ServerAPIClient, client =>
        {
            var baseAddress = configuration.GetValue<string>(AppsettingsKeysConstants.ServerAPIBaseAddress) ?? throw new InvalidOperationException($"Не задана настройка '{AppsettingsKeysConstants.ServerAPIBaseAddress}' в конфигурации.");
            client.BaseAddress = new Uri(baseAddress);
        });
        services.AddScoped<IServerAPIClient, ServerAPIClient>();

        services.AddHttpClient(ClientsConstants.IdentityServiceClient, client =>
        {
            var baseAddress = configuration.GetValue<string>(AppsettingsKeysConstants.IdentityServiceHTTPBaseAddress) ?? throw new InvalidOperationException($"Не задана настройка '{AppsettingsKeysConstants.IdentityServiceHTTPBaseAddress}' в конфигурации.");
            client.BaseAddress = new Uri(baseAddress);
        });
        services.AddScoped<IIndentityServiceClient, IndentityServiceClient>();
    }
}
