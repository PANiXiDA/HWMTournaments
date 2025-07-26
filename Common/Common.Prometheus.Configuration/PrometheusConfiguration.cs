using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Prometheus;
using Prometheus.DotNetRuntime;
using Prometheus.SystemMetrics;

namespace Common.Prometheus.Configuration;

public static class PrometheusConfiguration
{
    public static void AddPrometheusMetrics(this IServiceCollection services)
    {
        services.AddHealthChecks().ForwardToPrometheus();
        services.AddSystemMetrics();
        services.AddSingleton(DotNetRuntimeStatsBuilder.Default().StartCollecting());
    }

    public static void UsePrometheusMetrics(this WebApplication app)
    {
        app.UseHttpMetrics();
        app.MapMetrics();

        var collector = app.Services.GetRequiredService<IDisposable>();
        app.Lifetime.ApplicationStopping.Register(collector.Dispose);
    }
}

