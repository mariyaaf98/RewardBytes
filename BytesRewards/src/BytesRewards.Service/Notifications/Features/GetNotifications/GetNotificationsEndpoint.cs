using FastEndpoints;
using BytesRewards.Service.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BytesRewards.Service.Notifications.Features.GetNotifications;

public sealed class GetNotificationsEndpoint(ApplicationDbContext context)
    : EndpointWithoutRequest<List<NotificationDto>>
{
    // Notifications older than this are silently deleted on each fetch
    private static readonly TimeSpan RetentionPeriod = TimeSpan.FromDays(30);

    public override void Configure()
    {
        Get("/notifications");
        Roles("admin", "manager", "employee");
        Options(o => o.WithTags("13 - Notifications"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var keycloakId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(keycloakId))
        {
            Response = [];
            return;
        }

        // Try to find user by KeycloakUserId
        var user = await context.Users
            .FirstOrDefaultAsync(x => x.KeycloakUserId == keycloakId, ct);

        // If no User row exists (e.g. admin created only in Keycloak, not in Users table)
        // return empty — notifications require a User record to receive them
        if (user is null)
        {
            Response = [];
            return;
        }

        // Purge notifications older than 30 days for this user
        var cutoff = DateTime.UtcNow - RetentionPeriod;
        await context.Notifications
            .Where(n => n.UserId == user.Id && n.CreatedAt < cutoff)
            .ExecuteDeleteAsync(ct);

        // Return the remaining notifications, newest first
        Response = await context.Notifications
            .Where(n => n.UserId == user.Id)
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .Select(n => new NotificationDto
            {
                Id        = n.Id,
                Type      = n.Type,
                Title     = n.Title,
                Message   = n.Message,
                IsRead    = n.IsRead,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync(ct);
    }
}
