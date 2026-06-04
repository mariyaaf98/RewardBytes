using Aspire.Hosting;

Environment.SetEnvironmentVariable("USING_ASPIRE", "true");

// Load environment variables from a .env file (for development scenarios)
AppWeaver.Helpers.AppConfigurationLoader.LoadEnv();

var builder = DistributedApplication.CreateBuilder(args);


// Add infrastructure services with custom credentials

IResourceBuilder<PostgresServerResource> postgres = null!;

if (!string.IsNullOrEmpty(
            Environment.GetEnvironmentVariable("CUSTOM_DB_USERNAME")) &&
        !string.IsNullOrEmpty(
            Environment.GetEnvironmentVariable("CUSTOM_DB_PASSWORD")))
{
    var customUser = Environment.GetEnvironmentVariable("CUSTOM_DB_USERNAME");
    var customPassword = Environment.GetEnvironmentVariable("CUSTOM_DB_PASSWORD");
    var dbUser = builder.AddParameter("db-user", value: customUser!);
    var dbPassword = builder.AddParameter("db-password", value: customPassword!);

    postgres = builder.AddPostgres("postgres", dbUser, dbPassword)
        .WithDataVolume()
        .WithPgAdmin();
}
else
{
    postgres = builder.AddPostgres("postgres")
        //.WithDataVolume()
        .WithPgAdmin();    
}

var redis = builder.AddRedis("redis")
    .WithDataVolume()
    .WithRedisCommander();

// Add the main database
var database = postgres.AddDatabase("bytesrewards-db");

// Add the main API project with service discovery and observability
var api = builder.AddProject<Projects.BytesRewards_Service>("api")
    .WithReference(database)
    .WithReference(redis)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4317")
    //.WithHttpsEndpoint(port: 7001, name: "https")
    .WithHttpEndpoint(port: 7000, name: "http");
    // HTTPS disabled to avoid certificate issues in development
    // To enable HTTPS: .WithHttpsEndpoint(port: 7001, name: "https")

// Configure health checks endpoint
// Health check will be available at /health via MapDefaultEndpoints()
// api.WithHealthCheck("/health");

// Add external service dependencies (examples)
// Uncomment and configure as needed for your specific requirements

// SQL Server (alternative to PostgreSQL)
// var sqlserver = builder.AddSqlServer("sqlserver")
//     .WithDataVolume();
// var sqlDatabase = sqlserver.AddDatabase("appweaver-sqldb");

// MongoDB for document storage
// var mongodb = builder.AddMongoDB("mongodb")
//     .WithDataVolume()
//     .WithMongoExpress();

// RabbitMQ for messaging
// var rabbitmq = builder.AddRabbitMQ("rabbitmq")
//     .WithDataVolume()
//     .WithManagementPlugin();

// Elasticsearch for logging and search
// var elasticsearch = builder.AddElasticsearch("elasticsearch")
//     .WithDataVolume();

// Jaeger for distributed tracing
// var jaeger = builder.AddJaeger("jaeger");

// Prometheus for metrics
// var prometheus = builder.AddPrometheus("prometheus");

// Grafana for dashboards
// var grafana = builder.AddGrafana("grafana");

// Build and run the application
var app = builder.Build();

await app.RunAsync();
