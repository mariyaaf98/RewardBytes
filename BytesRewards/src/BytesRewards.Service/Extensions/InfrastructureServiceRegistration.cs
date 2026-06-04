using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection.Extensions;
using BytesRewards.Service.Infrastructure;
using AppWeaver.Repository.Abstractions;
using AppWeaver.Repository.EfCore;
using AppWeaver.DataStore.Extensions;
using AppWeaver.DataStore.EfCore.Extensions;
using AppWeaver.DataStore.Options;
using AppWeaver.Auditing.Extensions;
using AppWeaver.Auditing.EfCore.Extensions;

namespace BytesRewards.Service.Extensions;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, IConfiguration config)
    {
        if (!services.Any(s => s.ServiceType ==
                        typeof(DbContextOptions<ApplicationDbContext>)))
        {
            // Ensure folder for SQLite file exists
            var connStr = config.GetConnectionString("SqliteConnection")!;
            var builder = new SqliteConnectionStringBuilder(connStr);
            var relativePath = builder.DataSource;
            var dbPath = Path.Combine(AppContext.BaseDirectory, relativePath);

            var folderPath = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(folderPath))
                Directory.CreateDirectory(folderPath);

            // Repository wiring via new AppWeaver.Repository.EfCore
            services.AddRepositories(builder =>
            {
                builder
                    .UseSingleDbContextEFRepositories<ApplicationDbContext>(db =>
                        db.UseSqlite($"Data Source={dbPath}")
                          // WHY: Migrations are generated against PostgreSQL (design-time factory
                          // uses Npgsql when USING_ASPIRE is set). At runtime we use SQLite, so
                          // EF Core detects a model-hash mismatch between the PostgreSQL-typed
                          // snapshot and the SQLite-typed runtime model and throws
                          // PendingModelChangesWarning. This is a known multi-provider pattern;
                          // suppress the warning so MigrateAsync() proceeds. The actual DDL in
                          // migration Up() is provider-agnostic — SQLite treats column type
                          // strings like 'uuid' / 'timestamptz' as type-affinity hints and
                          // stores values correctly via EF Core's SQLite type mappings.
                          .ConfigureWarnings(w =>
                              w.Ignore(RelationalEventId.PendingModelChangesWarning))
                    )
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

            // AppWeaver.DataStore (Shared mode) — single SQLite file holds infrastructure
            // tables for every component (Auditing, Tenancy, etc.).
            services
                .AddDataStore(
                    storeBuilder => storeBuilder.UseEfCoreDataStore(
                        (_, opts) => opts.UseSqlite($"Data Source={dbPath}",
                            x => x.MigrationsAssembly(
                                typeof(ApplicationDbContext).Assembly.FullName))),
                    options =>
                    {
                        options.Strategy    = DataStoreStrategy.Shared;
                        options.TablePrefix = "AppWeaver_";
                        options.AutoMigrate = true;
                    });

            // AppWeaver.Auditing — bind to the active DataStore so audit tables live
            // in the SharedComponentDbContext migration.
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
        }

        // Adapter to bridge Application's non-generic
        // IUnitOfWork to AppWeaver.Repository's generic one
        //services.TryAddScoped<IUnitOfWork, UnitOfWorkAdapter>();
        services.TryAddScoped(typeof(IUnitOfWork),
                        typeof(EfUnitOfWork<ApplicationDbContext>));

        // Use in-memory cache for development
        services.AddMemoryCache();

        // Add health checks for database and cache
        var healthChecksBuilder = services.AddHealthChecks();

        healthChecksBuilder
            .AddSqlite(
                connectionString: $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "..", "App_Data", "appweaver-modular-monolith.db")}",
                name: "sqlite",
                tags: ["ready"]);

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
