using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

using BytesRewards.Service.Infrastructure;

using Microsoft.EntityFrameworkCore;

namespace BytesRewards.Service.RewardCategories.Features.DeleteRewardCategory;

public sealed class DeleteRewardCategoryCommandHandler(
    ApplicationDbContext context)
    : ICommandHandler<DeleteRewardCategoryCommand, Result<bool>>
{
    public async ValueTask<Result<bool>> Handle(
        DeleteRewardCategoryCommand request,
        CancellationToken ct)
    {
        var category = await context.RewardCategories
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (category is null)
            return Result<bool>.Failure(
                new Error(
                    "reward_category.not_found",
                    "Reward category not found.",
                    404,
                    "reward-categories"));

        // Prevent delete when category is in use by any reward
        var isInUse = await context.Rewards
            .AnyAsync(x => x.RewardCategoryId == request.Id, ct);

        if (isInUse)
            return Result<bool>.Failure(
                new Error(
                    "reward_category.in_use",
                    "This category cannot be deleted because it is assigned to one or more rewards.",
                    409,
                    "reward-categories"));

        // Soft delete — audit fields updated
        category.IsActive = false;
        category.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }
}
