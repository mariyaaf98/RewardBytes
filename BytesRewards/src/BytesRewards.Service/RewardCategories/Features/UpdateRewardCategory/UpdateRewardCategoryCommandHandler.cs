using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

using BytesRewards.Service.Infrastructure;

using Microsoft.EntityFrameworkCore;

namespace BytesRewards.Service.RewardCategories.Features.UpdateRewardCategory;

public sealed class UpdateRewardCategoryCommandHandler(
    ApplicationDbContext context)
    : ICommandHandler<UpdateRewardCategoryCommand, Result<bool>>
{
    public async ValueTask<Result<bool>> Handle(
        UpdateRewardCategoryCommand request,
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

        // Unique name check — exclude the current record
        var nameExists = await context.RewardCategories
            .AnyAsync(x =>
                x.IsActive &&
                x.Id != request.Id &&
                x.Name.ToLower() == request.Name.Trim().ToLower(),
                ct);

        if (nameExists)
            return Result<bool>.Failure(
                new Error(
                    "reward_category.name_duplicate",
                    $"A reward category named \"{request.Name}\" already exists.",
                    409,
                    "reward-categories"));

        category.Name = request.Name.Trim();
        category.Description = request.Description.Trim();
        category.Bytes = request.Bytes;
        category.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }
}
