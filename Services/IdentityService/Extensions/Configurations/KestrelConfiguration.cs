using Common.Constants.ServiceConfiguration;

using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace IdentityService.Extensions.Configurations;

public static class KestrelConfiguration
{
    public static void ConfigureKestrelPorts(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(ServicePortsConstants.IdentityServiceHttpPort, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1;
            });

            options.ListenAnyIP(ServicePortsConstants.IdentityServiceGrpcPort, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });
        });
    }
}
