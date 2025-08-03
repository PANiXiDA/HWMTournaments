using System;

using Common.Constants.ServiceConfiguration;
using Common.Helpers;

using Gen.IdentityService.ApplicationUserService;

using Microsoft.Extensions.DependencyInjection;

namespace Dev.Template.AspNetCore.API.Extensions.Configurations;

public static class GrpcClientConfiguration
{
    public static void ConfigureGrpcClients(this IServiceCollection services)
    {
        ConfigureGrpcClient<ApplicationUserService.ApplicationUserServiceClient>(services, ServiceNamesConstants.IdentityService);
    }

    private static void ConfigureGrpcClient<TClient>(IServiceCollection services, string serviceName) where TClient : class
    {
        string url = UrlHelper.GetServiceUrl(serviceName);
        services.AddGrpcClient<TClient>(options => options.Address = new Uri(url));
    }
}
