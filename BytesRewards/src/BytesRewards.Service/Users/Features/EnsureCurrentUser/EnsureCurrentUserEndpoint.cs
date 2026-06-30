using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;
using BytesRewards.Service.Infrastructure;
using BytesRewards.Service.Users.Domain;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BytesRewards.Service.Users.Features.EnsureCurrentUser;

/// <summary>
/// Ensures a User row exists for the currently authenticated Keycloak user.
/// Called on app init for admin/manager users who may not have been added
/// via the normal employee creation flow but do exist in Keycloak.
/// Returns the existing User.Id or creates a minimal record if missing.
/// </summary>
public sealed class EnsureCurrentUserEndpoint(ApplicationDbContext context)
    : SecureFastEndpoint<EnsureCurrentUserRequest, Guid>
{
    public override void Configure()
    {
        Post("/users/me/ensure");
        Roles("admin", "manager", "employee");
        Options(o => o.WithTags("01 - Users"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy() => new()
    {
        SecurityLevel = SecurityLevel.Internal,
        CachePolicy   = CachePolicy.NoStore
    };

    protected override async Task<Result<Guid>> ExecuteAsync(
        EnsureCurrentUserRequest req, CancellationToken ct)
    {
        var keycloakId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(keycloakId))
            throw new Exception("Keycloak user ID not found in token.");

        // If user already exists, return their ID
        var existing = await context.Users
            .FirstOrDefaultAsync(x => x.KeycloakUserId == keycloakId, ct);
        if (existing is not null)
            return Result<Guid>.Ok(existing.Id);

        // Resolve name and email from JWT claims
        var name  = User.FindFirst("name")?.Value
                 ?? User.FindFirst(ClaimTypes.Name)?.Value
                 ?? "Unknown";
        var email = User.FindFirst("email")?.Value
                 ?? User.FindFirst(ClaimTypes.Email)?.Value
                 ?? string.Empty;

        var parts     = name.Split(' ', 2);
        var firstName = parts.Length > 0 ? parts[0] : name;
        var lastName  = parts.Length > 1 ? parts[1] : string.Empty;

        // Get the seed "Unassigned" designation ID
        var designationId = await context.Designations
            .Where(d => d.Name == "Unassigned")
            .Select(d => d.Id)
            .FirstOrDefaultAsync(ct);

        // Get any department as a fallback
        var departmentId = await context.Departments
            .Select(d => d.Id)
            .FirstOrDefaultAsync(ct);

        var user = new User
        {
            Id              = Guid.NewGuid(),
            EmployeeId      = $"SYS-{Random.Shared.Next(1000, 9999)}",
            FirstName       = firstName,
            LastName        = lastName,
            Email           = email,
            PhoneNumber     = string.Empty,
            ProfileImageUrl = string.Empty,
            IsActive        = true,
            KeycloakUserId  = keycloakId,
            DepartmentId    = departmentId,
            DesignationId   = designationId,
            CreatedAt       = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync(ct);

        return Result<Guid>.Ok(user.Id);
    }
}

public sealed class EnsureCurrentUserRequest { }
