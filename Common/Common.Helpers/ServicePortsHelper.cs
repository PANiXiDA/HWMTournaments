using System;
using System.Collections.Generic;

using Common.Constants.ServiceConfiguration;

namespace Common.Helpers;

public static class ServicePortsHelper
{
    private static readonly Dictionary<string, (int HttpPort, int GrpcPort)> PortsMap = new()
    {
        { ServiceNamesConstants.ApiGateway, (ServicePortsConstants.APIGatewayHttpPort, ServicePortsConstants.APIGatewayGrpcPort) },
        { ServiceNamesConstants.IdentityService, (ServicePortsConstants.IdentityServiceHttpPort, ServicePortsConstants.IdentityServiceGrpcPort) },
    };

    public static (int HttpPort, int GrpcPort) GetPorts(string serviceName)
    {
        if (PortsMap.TryGetValue(serviceName, out var ports))
        {
            return ports;
        }

        throw new ArgumentException($"Service {serviceName} is not configured.", nameof(serviceName));
    }
}
