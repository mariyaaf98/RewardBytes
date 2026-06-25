using FastEndpoints;
using BytesRewards.Service.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BytesRewards.Service.Notifications.Features.MarkNotificationsRead;

/// <summary>
/// PUT /notifications/mark-read
/// Body: { "ids": ["guid1","guid2"] } — empty ids array marks ALL as read.
/// </summary>
public sealed class MarkNotificationsReadEndpoint(ApplicationDbContext context)
    : Endpoint<MarkNotificationsReadRequest, bool>
{
    public override void Configure()
    {
        Put("/notifications/mark-read");
        Roles("admin", "manager", "employee");
        Options(o => o.WithTags("13 - Notifications"));
    }

    public override async Task HandleAsync(
        MarkNotificationsReadRequest req, CancellationToken ct)
    {
        var keycloakId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(keycloakId))
        {
            Response = false;
            return;
        }

        var user = await context.Users
            .FirstOrDefaultAsync(x => x.KeycloakUserId == keycloakId, ct);

        if (user is null)
        {
            Response = false;
            return;
        }

        var query = context.Notifications
            .Where(n => n.UserId == user.Id && !n.IsRead);

        if (req.Ids is { Count: > 0 })
            query = query.Where(n => req.Ids.Contains(n.Id));

        await query.ExecuteUpdateAsync(
            s => s.SetProperty(n => n.IsRead, true), ct);

        Response = true;
    }
}

public sealed class MarkNotificationsReadRequest
{
    public List<Guid> Ids { get; set; } = [];
}
