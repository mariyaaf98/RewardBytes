using Microsoft.EntityFrameworkCore;
using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;
using BytesRewards.Service.Infrastructure;
using BytesRewards.Service.Infrastructure.Security.Keycloak;
using BytesRewards.Service.Notifications.Services;
using BytesRewards.Service.Redemptions.Domain;

namespace BytesRewards.Service.Redemptions.Features.RedeemReward;

public sealed class RedeemRewardCommandHandler(
    ApplicationDbContext context,
    NotificationService notifications,
    IKeycloakAdminService keycloakAdminService)
    : ICommandHandler<RedeemRewardCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(
        RedeemRewardCommand request,
        CancellationToken ct)
    {
        var wallet = await context.Wallets
            .FirstOrDefaultAsync(x => x.UserId == request.UserId, ct)
            ?? throw new Exception("Wallet not found");

        var rewardItem = await context.RewardItems
            .FirstOrDefaultAsync(x => x.Id == request.RewardItemId && x.IsActive, ct)
            ?? throw new Exception("Reward item not found");

        if (wallet.AvailableBytes < rewardItem.RequiredBytes)
            throw new Exception("Insufficient bytes");

        wallet.AvailableBytes -= rewardItem.RequiredBytes;

        var redemption = new Redemption
        {
            Id            = Guid.NewGuid(),
            UserId        = request.UserId,
            RewardItemId  = rewardItem.Id,
            RedeemedBytes = rewardItem.RequiredBytes,
            ProductName   = rewardItem.Name,
            Status        = "Pending",
            CreatedAt     = DateTime.UtcNow
        };

        context.Redemptions.Add(redemption);

        // ── Notify employee ──────────────────────────────────────
        notifications.Create(
            userId:  request.UserId,
            type:    "RedemptionPending",
            title:   $"🛒 Redemption submitted — {rewardItem.Name}",
            message: $"Your redemption request for \"{rewardItem.Name}\" " +
                     $"({rewardItem.RequiredBytes} bytes) is pending admin approval.");

        // ── Notify admins only ───────────────────────────────────
        var employee = await context.Users
            .Where(u => u.Id == request.UserId)
            .Select(u => new { FullName = u.FirstName + " " + u.LastName })
            .FirstOrDefaultAsync(ct);

        var adminIds = await GetAdminUserIdsAsync(ct);

        notifications.CreateForUsers(
            userIds: adminIds,
            type:    "NewRedemptionRequest",
            title:   $"🛒 New redemption request",
            message: $"{employee?.FullName ?? "An employee"} requested \"{rewardItem.Name}\" " +
                     $"({rewardItem.RequiredBytes} bytes). Pending your approval.");

        await context.SaveChangesAsync(ct);

        return Result<Guid>.Ok(redemption.Id);
    }

    /// <summary>
    /// Resolves all user IDs whose Keycloak role is "admin".
    /// Falls back to an empty list if Keycloak is unavailable.
    /// </summary>
    private async Task<List<Guid>> GetAdminUserIdsAsync(CancellationToken ct)
    {
        var users = await context.Users
            .Where(u => u.IsActive && u.KeycloakUserId != string.Empty)
            .Select(u => new { u.Id, u.KeycloakUserId })
            .ToListAsync(ct);

        string token;
        try   { token = await keycloakAdminService.GetAdminTokenAsync(ct); }
        catch { return []; }

        var adminIds = new List<Guid>();
        foreach (var user in users)
        {
            try
            {
                var role = await keycloakAdminService
                    .GetUserRoleAsync(token, user.KeycloakUserId, ct);
                if (string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase))
                    adminIds.Add(user.Id);
            }
            catch { /* skip if role lookup fails for this user */ }
        }

        return adminIds;
    }
}
