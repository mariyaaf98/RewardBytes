using Microsoft.EntityFrameworkCore;
using AppWeaver.Mediator.Interfaces;
using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.Rewards.Features.GetEmployeeRewardStatus;

public sealed class GetEmployeeRewardStatusQueryHandler(
    ApplicationDbContext context)
    : IQueryHandler<GetEmployeeRewardStatusQuery, EmployeeRewardStatusResponse>
{
    public async ValueTask<EmployeeRewardStatusResponse> Handle(
        GetEmployeeRewardStatusQuery request,
        CancellationToken ct)
    {
        var cutoff = DateTime.UtcNow.AddMonths(-6);

        // Load all active users — no Keycloak call needed
        var users = await context.Users
            .Where(u => u.IsActive)
            .Include(u => u.Department)
            .Include(u => u.Designation)
            .Select(u => new
            {
                u.Id,
                FullName        = u.FirstName + " " + u.LastName,
                DepartmentName  = u.Department.Name,
                DesignationName = u.Designation != null ? u.Designation.Name : string.Empty
            })
            .ToListAsync(ct);

        // Aggregate rewards per recipient within the 6-month window
        var rewardGroups = await context.Rewards
            .Where(r => r.CreatedAt >= cutoff)
            .GroupBy(r => r.ToUserId)
            .Select(g => new
            {
                ToUserId      = g.Key,
                Count         = g.Count(),
                LastAwardedAt = g.Max(r => r.CreatedAt)
            })
            .ToListAsync(ct);

        // Fetch the most-recent reward row per user to get category + bytes
        var mostRecentRewards = await context.Rewards
            .Where(r => r.CreatedAt >= cutoff)
            .OrderByDescending(r => r.CreatedAt)
            .GroupBy(r => r.ToUserId)
            .Select(g => new
            {
                ToUserId             = g.Key,
                RewardCategoryName   = g.OrderByDescending(r => r.CreatedAt)
                                        .Select(r => r.RewardCategoryName)
                                        .First(),
                Bytes                = g.OrderByDescending(r => r.CreatedAt)
                                        .Select(r => r.Bytes)
                                        .First()
            })
            .ToListAsync(ct);

        var rewardLookup      = rewardGroups.ToDictionary(r => r.ToUserId);
        var mostRecentLookup  = mostRecentRewards.ToDictionary(r => r.ToUserId);

        var rewarded    = new List<EmployeeRewardSummary>();
        var notRewarded = new List<EmployeeRewardSummary>();

        foreach (var user in users)
        {
            if (rewardLookup.TryGetValue(user.Id, out var rg))
            {
                mostRecentLookup.TryGetValue(user.Id, out var mr);
                rewarded.Add(new EmployeeRewardSummary
                {
                    Id                      = user.Id,
                    FullName                = user.FullName,
                    DepartmentName          = user.DepartmentName,
                    DesignationName         = user.DesignationName,
                    LastRewardedAt          = rg.LastAwardedAt,
                    LastRewardCategoryName  = mr?.RewardCategoryName ?? string.Empty,
                    LastRewardBytes         = mr?.Bytes ?? 0,
                    TotalRewards            = rg.Count
                });
            }
            else
            {
                notRewarded.Add(new EmployeeRewardSummary
                {
                    Id              = user.Id,
                    FullName        = user.FullName,
                    DepartmentName  = user.DepartmentName,
                    DesignationName = user.DesignationName
                });
            }
        }

        return new EmployeeRewardStatusResponse
        {
            Rewarded    = [.. rewarded.OrderByDescending(e => e.LastRewardedAt)],
            NotRewarded = [.. notRewarded.OrderBy(e => e.FullName)]
        };
    }
}
