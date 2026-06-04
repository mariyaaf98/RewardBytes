using AppWeaver.Helpers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BytesRewards.Service.Infrastructure;

/// <summary>
/// Design-time factory for ApplicationDbContext that supports both Aspire (PostgreSQL) and regular (SQLite) modes.
/// Simple approach: Uses USING_ASPIRE environment variable to determine database provider.
/// - USING_ASPIRE=true → PostgreSQL (Aspire mode)
/// - USING_ASPIRE not set → SQLite (Regular mode)
/// </summary>
public class ApplicationDbContextFactory : 
            IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Load environment variables from .env file
        AppConfigurationLoader.LoadEnv();

        // Build configuration from multiple sources
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddJsonFile("appsettings.Aspire.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Simple detection: Check if running with Aspire
        var isAspire = !string.IsNullOrEmpty(
            Environment.GetEnvironmentVariable("USING_ASPIRE"));

        if (isAspire)
        {
            // PostgreSQL for Aspire mode
            ConfigurePostgreSql(optionsBuilder, config);
            EfLogger.Log("Using PostgreSQL (Aspire mode)");
        }
        else
        {
            // SQLite for regular mode
            ConfigureSqlite(optionsBuilder, config);
            EfLogger.Log("Using SQLite (Regular mode)");
        }

        return new ApplicationDbContext(optionsBuilder.Options);
    }

    /// <summary>
    /// Configures PostgreSQL for Aspire mode.
    /// </summary>
    private static void ConfigurePostgreSql(
        DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder,
                                          IConfiguration config)
    {
        // Try to get Aspire connection string first
        var connectionString =
            config.GetConnectionString("bytesrewards-db");
        
        // Fallback to environment variable
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString =
                Environment.GetEnvironmentVariable(
                    "ConnectionStrings__bytesrewards-db");
        }

        // Final fallback to a default PostgreSQL connection string for migrations
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString =
                "Host=localhost;Port=5432;Database=bytesrewards-db;Username=myuser;Password=mysecurepassword";
        }

        EfLogger.Log(
            $"PostgreSQL Connection: {MaskConnectionString(connectionString)}");
        
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

    /// <summary>
    /// Configures SQLite for regular mode.
    /// </summary>
    private static void ConfigureSqlite(
        DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder,
                                      IConfiguration config)
    {

        // SQLite configuration
        var connStr = config.GetConnectionString("SqliteConnection")!;
        var builder = new SqliteConnectionStringBuilder(connStr);
        var relativePath = builder.DataSource;
        var dbPath = Path.Combine(AppContext.BaseDirectory, relativePath);

        EfLogger.Log($"SQLite Database: {dbPath}");

        var folderPath = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(folderPath))
                Directory.CreateDirectory(folderPath);

        //optionsBuilder.UseSqlite($"Data Source={dbPath}");
        
        optionsBuilder.UseSqlite($"Data Source={dbPath}", options =>
        {
            options.MigrationsAssembly(
                    typeof(ApplicationDbContext).Assembly.FullName);
        });
    }

    /// <summary>
    /// Masks sensitive information in connection strings for logging.
    /// </summary>
    public static string MaskConnectionString(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString)) return "Not configured";

        // Simple masking for common password patterns
        var masked = connectionString;
        var patterns = new[] { "Password=", "Pwd=", "password=" };
        
        foreach (var pattern in patterns)
        {
            var index = masked.IndexOf(
                pattern, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                var start = index + pattern.Length;
                var end = masked.IndexOf(';', start);
                if (end == -1) end = masked.Length;
                
                var passwordLength = end - start;
                if (passwordLength > 0)
                {
                    masked = masked[..start] + new string(
                            '*', Math.Min(passwordLength, 8)) + 
                            (end < masked.Length ? masked[end..] : "");
                }
            }
        }
        
        return masked;
    }
}
