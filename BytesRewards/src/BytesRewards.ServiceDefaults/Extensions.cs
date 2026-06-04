using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Provides extension methods for configuring AppWeaver service defaults with .NET Aspire.
/// This includes observability, resilience, service discovery, and health checks.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds AppWeaver service defaults including observability, resilience, and service discovery.
    /// </summary>
    /// <param name="builder">The host application builder</param>
    /// <returns>The configured host application builder</returns>
    public static IHostApplicationBuilder AddServiceDefaults(
                                this IHostApplicationBuilder builder )
    {
        builder.ConfigureOpenTelemetry();
        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();
        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler(); // Turn on resilience by default
            http.AddServiceDiscovery(); // Turn on service discovery by default
        });

        // Add AppWeaver-specific configurations
        builder.AddAppWeaverDefaults();

        return builder;
    }

    /// <summary>
    /// Configures OpenTelemetry for comprehensive observability.
    /// </summary>
    /// <param name="builder">The host application builder</param>
    /// <returns>The configured host application builder</returns>
    public static IHostApplicationBuilder ConfigureOpenTelemetry(
                                    this IHostApplicationBuilder builder)
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter("BytesRewards.*") //Refer the Project Services
                    .AddMeter("AppWeaver.Repository")
                    .AddMeter("AppWeaver.Mediator"); //Add Required AppWeaver Components
            })
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation(options =>
                    {
                        options.SetDbStatementForText = true;
                        options.SetDbStatementForStoredProcedure = true;
                        options.EnrichWithIDbCommand = (activity, command) =>
                        {
                            activity.SetTag("db.command_timeout", command.CommandTimeout);
                        };
                    })
                    .AddSource("BytesRewards.*")   //Refer the Project Services
                    .AddSource("AppWeaver.Repository")
                    .AddSource("AppWeaver.Mediator");   //Add Required AppWeaver Components
            });

        // Auto-export to Aspire’s OTEL collector if available
        var otelEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
        if (!string.IsNullOrWhiteSpace(otelEndpoint))
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        return builder;
    }

    /// <summary>
    /// Adds default health checks for the application.
    /// </summary>
    /// <param name="builder">The host application builder</param>
    /// <returns>The configured host application builder</returns>
    public static IHostApplicationBuilder AddDefaultHealthChecks(
                                    this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    /// <summary>
    /// Maps health check endpoints with proper configuration.
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The configured web application</returns>
    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Configure health check endpoints
        app.MapHealthChecks("/health");
        app.MapHealthChecks("/alive", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live")
        });

        return app;
    }

    /// <summary>
    /// Adds AppWeaver-specific service defaults.
    /// </summary>
    /// <param name="builder">The host application builder</param>
    /// <returns>The configured host application builder</returns>
    private static IHostApplicationBuilder AddAppWeaverDefaults(
                                    this IHostApplicationBuilder builder)
    {
        // Configure structured logging with correlation IDs
        builder.Services.Configure<LoggerFilterOptions>(options =>
        {
            // Reduce noise from Entity Framework
            options.AddFilter(
                    "Microsoft.EntityFrameworkCore.Database.Command",
                    LogLevel.Warning);
            options.AddFilter(
                    "Microsoft.EntityFrameworkCore.Infrastructure",
                    LogLevel.Warning);
            
            // Reduce noise from ASP.NET Core
            options.AddFilter(
                    "Microsoft.AspNetCore.Hosting.Diagnostics",
                    LogLevel.Warning);
            options.AddFilter(
                    "Microsoft.AspNetCore.Routing.EndpointMiddleware",
                    LogLevel.Warning);
        });

        return builder;
    }
}
