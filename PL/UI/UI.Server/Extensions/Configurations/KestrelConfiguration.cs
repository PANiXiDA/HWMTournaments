using Common.Constants.ServiceConfiguration;

using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace UI.Server.Extensions.Configurations;

public static class KestrelConfiguration
{
    public static void ConfigureKestrelPorts(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(ServicePortsConstants.APIGatewayHttpPort, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1;
            });

            options.ListenAnyIP(ServicePortsConstants.APIGatewayGrpcPort, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });
        });
    }
}
