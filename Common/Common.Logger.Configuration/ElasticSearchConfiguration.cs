using System.Text.RegularExpressions;

using Microsoft.Extensions.Configuration;

using Serilog;
using Serilog.Events;
using Serilog.Sinks.OpenSearch;

namespace Common.Logger.Configuration;

public static class ElasticSearchConfiguration
{
    public static ILogger ConfigureElasticSearch(string indexPrefix, IConfiguration configuration)
    {
        indexPrefix = Regex.Replace(indexPrefix, "(?<=[a-z0-9])(?=[A-Z])", "-").Replace(".", "-").ToLower();
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower().Replace(".", "-");
        var serviceName = configuration["ElasticSearch:ServiceName"];

        var serilog = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.WithProperty("service_name", serviceName)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .WriteTo.Console()
            .WriteTo.Debug()
            .WriteTo.OpenSearch(new OpenSearchSinkOptions(new Uri(configuration["ElasticSearch:Url"]!))
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.OSv1,
                IndexFormat = $"{indexPrefix}-{environment}",
                ModifyConnectionSettings = x => x
                    .BasicAuthentication(configuration["ElasticSearch:Login"], configuration["ElasticSearch:Password"])
                    .ThrowExceptions(true),
            })
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        return serilog;
    }
}


