using BytesRewards.Service.Infrastructure;
using BytesRewards.Service.Notifications.Domain;
using Microsoft.EntityFrameworkCore;

namespace BytesRewards.Service.Notifications.Services;

/// <summary>
/// Writes notification rows inside the same EF Core SaveChanges call as the originating command.
/// Caller is always responsible for calling SaveChangesAsync after this.
/// </summary>
public sealed class NotificationService(ApplicationDbContext context)
{
    // ── Single user ──────────────────────────────────────────────
    public void Create(Guid userId, string type, string title, string message)
    {
        context.Notifications.Add(new Notification
        {
            Id        = Guid.NewGuid(),
            UserId    = userId,
            Type      = type,
            Title     = title,
            Message   = message,
            IsRead    = false,
            CreatedAt = DateTime.UtcNow
        });
    }

    // ── Explicit list of user IDs ────────────────────────────────
    /// <summary>
    /// Creates a notification for each user ID in the provided list.
    /// </summary>
    public void CreateForUsers(
        IEnumerable<Guid> userIds,
        string            type,
        string            title,
        string            message)
    {
        var now = DateTime.UtcNow;
        foreach (var uid in userIds)
        {
            context.Notifications.Add(new Notification
            {
                Id        = Guid.NewGuid(),
                UserId    = uid,
                Type      = type,
                Title     = title,
                Message   = message,
                IsRead    = false,
                CreatedAt = now
            });
        }
    }

    // ── All active users except specified ones ───────────────────
    /// <summary>
    /// Creates one notification row for every active user,
    /// skipping the IDs in <paramref name="excludeUserIds"/>.
    /// Pure DB query — no Keycloak call needed.
    /// </summary>
    public async Task CreateForAllUsersExceptAsync(
        IEnumerable<Guid> excludeUserIds,
        string            type,
        string            title,
        string            message,
        CancellationToken ct)
    {
        var exclude = excludeUserIds.ToHashSet();

        var userIds = await context.Users
            .Where(u => u.IsActive)
            .Select(u => u.Id)
            .ToListAsync(ct);

        var now = DateTime.UtcNow;
        foreach (var uid in userIds)
        {
            if (exclude.Contains(uid)) continue;

            context.Notifications.Add(new Notification
            {
                Id        = Guid.NewGuid(),
                UserId    = uid,
                Type      = type,
                Title     = title,
                Message   = message,
                IsRead    = false,
                CreatedAt = now
            });
        }
    }
}
