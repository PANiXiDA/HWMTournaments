using BL.Implementations;
using BL.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace BL.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection UseBL(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ISettingsBL, SettingsBL>();
        serviceCollection.AddScoped<IUsersBL, UsersBL>();

        return serviceCollection;
    }
}