using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Mondabet.Shared.Infrastructure.Observability;

public static class OpenTelemetryExtensions
{
    /// <summary>
    /// Registers OTLP tracing for the service. Call from each microservice's Program.cs.
    /// Reads "OpenTelemetry:Endpoint" (default http://otel-collector:4317) from configuration.
    /// </summary>
    public static IServiceCollection AddMondabetTracing(
        this IServiceCollection services, IConfiguration configuration, string serviceName)
    {
        var endpoint = configuration["OpenTelemetry:Endpoint"] ?? "http://otel-collector:4317";

        services.AddOpenTelemetry()
            .ConfigureResource((r) => r.AddService(serviceName))
            .WithTracing((builder) =>
            {
                builder
                    .AddAspNetCoreInstrumentation(o =>
                    {
                        o.RecordException = true;
                        o.Filter = (ctx) => !ctx.Request.Path.StartsWithSegments("/health");
                    })
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation(o => o.SetDbStatementForText = true)
                    .AddOtlpExporter(o => o.Endpoint = new Uri(endpoint));
            });

        return services;
    }
}
