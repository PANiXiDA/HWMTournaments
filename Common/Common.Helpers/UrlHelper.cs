using Common.Constants.ServiceConfiguration;

namespace Common.Helpers;

public static class UrlHelper
{
    public static string GetServiceUrl(string serviceName, bool useGrpcPort = true)
    {
        var (httpPort, grpcPort) = ServicePortsHelper.GetPorts(serviceName);
        var port = useGrpcPort ? grpcPort : httpPort;

        var environmentSuffix = EnvironmentHelper.IsDevelopment ? ServiceNamesConstants.DevelopmentSuffix : ServiceNamesConstants.ProductionSuffix;
        var host = EnvironmentHelper.IsLocalHost
            ? ServiceNamesConstants.LocalHost
            : $"{serviceName}-{environmentSuffix}";

        return $"{ServiceNamesConstants.HttpScheme}{host}:{port}";
    }
}
