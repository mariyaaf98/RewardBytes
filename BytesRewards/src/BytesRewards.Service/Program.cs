using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using BytesRewards.Service.Infrastructure.Security.Keycloak;

using Serilog;

using BytesRewards.Service.Extensions;
using BytesRewards.Service.Infrastructure;

using AppWeaver.Api.Middlewares.ExceptionHandler.Extensions;
using AppWeaver.ResultMapping;
using AppWeaver.Contexts;




using System.Security.Claims;
using System.Text.Json;

// using AppWeaver.Security.Extensions;
// using AppWeaver.Security.Authentication.AzureADB2C.Extensions;
// using AppWeaver.Security.Authentication.EntraExternalID.Extensions;
// using AppWeaver.Security.Services;

//Is running with Aspire
var isAspire = string.Equals(
        Environment.GetEnvironmentVariable("USING_ASPIRE"), "true",
        StringComparison.OrdinalIgnoreCase);

// Load environment variables from a .env file (for development scenarios)
if (!isAspire)
    AppWeaver.Helpers.AppConfigurationLoader.LoadEnv();




var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults
builder.AddServiceDefaults();

builder.Services.Configure<KeycloakAdminOptions>(
    builder.Configuration.GetSection("KeycloakAdmin"));

builder.Services.AddHttpClient<
    IKeycloakAdminService,
    KeycloakAdminService>();

// Setup Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Configuration
var cfg = builder.Configuration;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority =
            "http://localhost:8080/realms/bytes-rewards";

        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters.ValidateAudience = false;

        options.TokenValidationParameters.NameClaimType =
            "preferred_username";

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var identity =
                    context.Principal?.Identity as ClaimsIdentity;

                var realmAccess =
                    context.Principal?.FindFirst("realm_access")?.Value;

                if (!string.IsNullOrEmpty(realmAccess))
                {
                    var roles = JsonDocument
                        .Parse(realmAccess)
                        .RootElement
                        .GetProperty("roles");

                    foreach (var role in roles.EnumerateArray())
                    {
                        identity?.AddClaim(
                            new Claim(
                                ClaimTypes.Role,
                                role.GetString()!
                            )
                        );
                    }
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();



// #region [ Use For Authentication & Authorization ]
// // Determine which auth provider to use (default: AzureAdB2C)
// var authProvider = cfg["AuthProvider"] ?? "AzureAdB2C";

// // Security composition with Azure AD B2C
// // In this scenario:
// // - Azure AD B2C handles ALL identity management (user storage, registration, password reset)
// // - We only need Authentication (token validation) and Authorization (RBAC)
// // - NO local identity provider needed - Azure AD B2C is the source of truth
// builder.Services.AddAppWeaverSecurity(security =>
// {
//     security
//         // NO .AddIdentity( ) //- Azure AD B2C manages users, we don't store them locally
//         .AddAuthentication(auth =>
//         {
//             // Use Azure AD B2C authentication provider
//             // This validates JWT tokens issued by Azure AD B2C
//             if (authProvider == "EntraExternalID")
//             {
//                 // Use Microsoft Entra External ID (CIAM) authentication provider
//                 auth.UseEntraExternalID(o =>
//                 {
//                     o.TenantSubdomain = cfg["EntraExternalID:TenantSubdomain"]!;
//                     o.TenantId = cfg["EntraExternalID:TenantId"]!;
//                     o.ClientId = cfg["EntraExternalID:ClientId"]!;
//                 }, cfg);
//             }
//             else
//             auth.UseAzureAdB2C(o =>
//             {
//                 o.Instance = cfg["AzureAdB2C:Instance"]!;
//                 o.Domain = cfg["AzureAdB2C:Domain"]!;
//                 o.ClientId = cfg["AzureAdB2C:ClientId"]!;
//                 o.SignUpSignInPolicyId = cfg["AzureAdB2C:SignUpSignInPolicyId"]!;
//             }, cfg);
//         })
//         .AddAuthorization(authz =>
//         {
//             // Use RBAC authorization based on roles from Azure AD B2C token
//             authz.UseRoleBased(o =>
//             {
//                 o.AdminRole = "Admin";
//             });
//         });
// },
// options =>
// {
//     // Configure security options
// },
// configuration: cfg);

// #endregion

// Setup centralized exception handling from configuration (uses AppWeaver.ProblemDetails)
builder.Services.AddExceptionHandling(
    builder.Configuration,
    configureLogger: logger =>
    {
        logger.LogInnerException = false;
        logger.LogLevel = LogLevel.Information.ToString();
        logger.LogStackTrace = false;
        logger.LogTraceId = true;
        logger.LogCorrelationId = true;
    }
);



// Configure CORS for Aspire and development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AspirePolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:15000", // Aspire Dashboard
                "http://localhost:7000",  // API HTTP
                "http://localhost:4200",  // Angular Frontend
                "http://localhost:3000"   // React dev server
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });

    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:7000",  // API HTTP
                "http://localhost:4200",  // Angular Frontend
                "http://localhost:3000"   // React dev server
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Register core FastEndpoints and Swagger support
builder.Services.AddFastEndpoints();
builder.Services.AddResultMapping(); // AppWeaver.Results

// Add Swagger with JWT Bearer authentication support (lock icon in Swagger UI)
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "BytesRewards API";
        s.Version = "v1";
        // s.Description = "BytesRewards API with Azure AD B2C authentication";
        s.Description = "BytesRewards API with Keycloak authentication";

        // Add JWT Bearer authentication to Swagger
        s.AddAuth("Bearer", new()
        {
            Type = NSwag.OpenApiSecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            // Description = "Enter your Azure AD B2C JWT token"
            Description = "Enter your Keycloak JWT token"
        });
    };

    o.AutoTagPathSegmentIndex = 0;
    o.ShortSchemaNames = true;
});

// Register default contexts (user/tenant) and repositories via new stack
builder.Services.RegisterDefaultContexts(
    userId: "api-user", userName: "api-user", displayName: "API User",
    isAuthenticated: true, tenantId: "tenant-1", tenantName: "Tenant 1",
    tenantResolved: true);

// Register infrastructure and application services (DbContext via Infrastructure)
// Use Aspire infrastructure for full integration with Infra services
if (isAspire)
    builder.Services.AddAspireInfrastructure(builder.Configuration);
else // Use local infrastructure for development (SQLite, in-memory cache)
    builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApplication();

var app = builder.Build();

// Configure HTTP request pipeline
// NOTE: Exception handler must be registered early to capture exceptions from endpoints
app.UseExceptionHandler(); // Custom error formatting

// Configure CORS based on environment
if (isAspire)
{
    app.UseCors("AspirePolicy");
}
else
{
    app.UseCors("DevelopmentPolicy");
}

// Run EF Core migrations at startup (DevOps shortcut)
await app.MigrateDatabaseAsync<ApplicationDbContext>();
await DataSeeder.SeedAsync(app.Services);

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints();
app.UseSwaggerGen();

// Map Aspire health check and observability endpoints
app.MapDefaultEndpoints();

//// AppWeaver Template Endpoints

// Redirect root to Swagger for easy API discovery
app.MapGet("/", () => Results.Redirect("/swagger"))
    .WithName("RedirectToSwagger")
    .WithSummary("Redirect to Swagger UI")
    .ExcludeFromDescription();

if (!app.Environment.IsEnvironment("Development"))
    app.UseHttpsRedirection(); // Enforce HTTPS outside development

await app.RunAsync(); // Launch the application

public partial class Program { } // for WebApplicationFactory
