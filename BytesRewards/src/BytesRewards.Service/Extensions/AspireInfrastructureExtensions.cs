using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using BytesRewards.Service.Infrastructure;
using AppWeaver.Repository.EfCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using AppWeaver.Repository.Abstractions;
using StackExchange.Redis;
using AppWeaver.DataStore.Extensions;
using AppWeaver.DataStore.EfCore.Extensions;
using AppWeaver.DataStore.Options;
using AppWeaver.Auditing.Extensions;
using AppWeaver.Auditing.EfCore.Extensions;

namespace BytesRewards.Service.Extensions;

/// <summary>
/// Infrastructure service registration extensions with Aspire integration support.
/// Provides flexible database configuration for development (SQLite) and production (PostgreSQL via Aspire).
/// </summary>
public static class AspireInfrastructureExtensions
{
    /// <summary>
    /// Registers infrastructure services with Aspire integration support.
    /// Uses PostgreSQL when Aspire connection string is available, falls back to SQLite for development.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The configured service collection</returns>
    public static IServiceCollection AddAspireInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Check if we're running with Aspire (connection string will be injected)
        var aspireConnectionString =
                configuration.GetConnectionString("bytesrewards-db");

        // Repository wiring via AppWeaver.Repository.EfCore
        // Use existing DbContext registration from above
        services.AddRepositories(builder =>
        {
            builder
                .UseSingleDbContextEFRepositories<ApplicationDbContext>(db =>
                    db.UseNpgsql(aspireConnectionString))
                .EnableAuditing() //- Use in case of Basic UseEfCoreAuditing
                .EnableTenancy()
                .EnableSoftDelete()
                .EnableAdvancedAuditing()
                .EnableClientManagedConcurrency()
                .SetQueryTrackingBehavior(noTracking: false)
                .Done();
        }, options =>
        {
            options.Provider = "EfCore";
        });

        // Bridge for EfRepository<T> which depends on DbContext
        services.AddScoped<DbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        // AppWeaver.DataStore (Shared mode) — Auditing tables share the project database.
        services
            .AddDataStore(
                storeBuilder => storeBuilder.UseEfCoreDataStore(
                    (_, opts) => opts.UseNpgsql(aspireConnectionString,
                        x => x.MigrationsAssembly(
                            typeof(ApplicationDbContext).Assembly.FullName))),
                options =>
                {
                    options.Strategy    = DataStoreStrategy.Shared;
                    options.TablePrefix = "AppWeaver_";
                    options.AutoMigrate = true;
                });

        // AppWeaver.Auditing wired to the shared DataStore.
        services.AddAuditing(
            auditBuilder =>
            {
                auditBuilder
                    .UseEfCoreAuditing(o =>
                    {
                        o.MaxPropertyValueLength  = 4000;
                        o.ExcludeShadowProperties = true;
                        o.TablePrefix             = "Audit_";
                        o.SchemaName              = "Auditing";
                    })
                    .UseAppWeaverDataStore();
            },
            options =>
            {
                options.Provider                = "EfCore";
                options.IsEnabled               = true;
                options.EnableStreaming         = false;
                options.EnableFieldLevelAuditing = true;
            });

        // Add Redis caching with Aspire
        // Unit of Work registration
        services.TryAddScoped(typeof(IUnitOfWork),
            typeof(EfUnitOfWork<ApplicationDbContext>));

        services.AddStackExchangeRedisCache(options =>
        {
            var redisConnectionString = configuration.GetConnectionString("redis");
            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                options.Configuration = redisConnectionString;
            }
        });

        // Register Redis connection multiplexer for advanced scenarios
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var redisConnectionString = configuration.GetConnectionString("redis");
            return ConnectionMultiplexer.Connect(redisConnectionString ?? "localhost:6379");
        });


        // Add health checks for database and cache
        var healthChecksBuilder = services.AddHealthChecks();

        if (!string.IsNullOrEmpty(aspireConnectionString))
        {
            healthChecksBuilder.AddNpgSql(
                connectionString: aspireConnectionString,
                name: "postgresql",
                tags: ["ready"]);
        }

        var redisConnectionString = configuration.GetConnectionString("redis");
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            healthChecksBuilder.AddRedis(
                redisConnectionString,
                name: "redis",
                tags: ["ready"]);
        }

        return services;
    }

    /// <summary>
    /// Determines if the application is running with Aspire orchestration.
    /// </summary>
    /// <param name="configuration">The configuration</param>
    /// <returns>True if running with Aspire, false otherwise</returns>
    public static bool IsRunningWithAspire(this IConfiguration configuration)
    {
        return string.Equals(
            Environment.GetEnvironmentVariable("USING_ASPIRE"), "true",
            StringComparison.OrdinalIgnoreCase);
    }
}
