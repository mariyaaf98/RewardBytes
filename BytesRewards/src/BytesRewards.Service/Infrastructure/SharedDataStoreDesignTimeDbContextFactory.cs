using AppWeaver.Auditing.EfCore.Migrations;
using AppWeaver.DataStore.EfCore.Contexts;
using AppWeaver.DataStore.EfCore.Migrations;
using AppWeaver.DataStore.Options;
using AppWeaver.Helpers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace BytesRewards.Service.Infrastructure;

/// <summary>
/// Design-time factory for <see cref="SharedComponentDbContext"/>.
///
/// Mirrors <see cref="ApplicationDbContextFactory"/> so migrations for the project DB
/// and the DataStore SharedComponentDbContext target the same provider and database.
///
/// Discovery: EF Core picks up this factory automatically when you run:
/// <code>
///   dotnet ef migrations add InitialDataStore \
///     --context SharedComponentDbContext \
///     --output-dir Migrations/DataStore
///
///   dotnet ef database update --context SharedComponentDbContext
/// </code>
/// </summary>
public class SharedDataStoreDesignTimeDbContextFactory : 
                                    SharedDesignTimeDbContextFactory
{
    protected override void ConfigureProvider(
        DbContextOptionsBuilder<SharedComponentDbContext> optionsBuilder )
    {
        AppConfigurationLoader.LoadEnv();

        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddJsonFile("appsettings.Aspire.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var isAspire = !string.IsNullOrEmpty(
            Environment.GetEnvironmentVariable("USING_ASPIRE"));

        if (isAspire)
        {
            var connectionString =
                config.GetConnectionString("bytesrewards-db")
                ?? Environment.GetEnvironmentVariable(
                    "ConnectionStrings__bytesrewards-db")
                ?? "Host=localhost;Port=5432;Database=bytesrewards-db;Username=myuser;Password=mysecurepassword";

            EfLogger.Log(
                $"PostgreSQL Connection For DataStore: {
                    ApplicationDbContextFactory.MaskConnectionString(connectionString)}");

            optionsBuilder.UseNpgsql(connectionString, options =>
            {
                options.MigrationsAssembly(
                    typeof(ApplicationDbContext).Assembly.FullName);
                options.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            });
        }
        else
        {
            var connStr = config.GetConnectionString("SqliteConnection")!;
            var dbPath = Path.Combine(
                AppContext.BaseDirectory,
                new SqliteConnectionStringBuilder(connStr).DataSource);

            var folderPath = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(folderPath))
                Directory.CreateDirectory(folderPath);

            optionsBuilder.UseSqlite($"Data Source={dbPath}", options =>
                options.MigrationsAssembly(
                    typeof(ApplicationDbContext).Assembly.FullName));
        }

        // Migrations are generated against PostgreSQL when USING_ASPIRE is set; SQLite at
        // runtime sees a PendingModelChangesWarning due to provider-specific column types.
        // The DDL is provider-agnostic — suppress the warning so updates apply cleanly.
        optionsBuilder.ConfigureWarnings(w =>
            w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override IEnumerable<ComponentEntityMetadata> GetComponentMetadata()
    {
        // Same TablePrefix / SchemaName values used in InfrastructureServiceRegistration.
        yield return AuditingDesignTimeHelper.CreateMetadata(
            tablePrefix: "Audit_",
            schemaName:  "Auditing");

        //Provide other components if any
    }
}
