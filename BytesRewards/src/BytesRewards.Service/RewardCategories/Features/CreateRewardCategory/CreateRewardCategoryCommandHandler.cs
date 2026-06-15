using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

using BytesRewards.Service.Infrastructure;
using BytesRewards.Service.RewardCategories.Domain;

using Microsoft.EntityFrameworkCore;

namespace BytesRewards.Service.RewardCategories.Features.CreateRewardCategory;

public sealed class CreateRewardCategoryCommandHandler(
    ApplicationDbContext context)
    : ICommandHandler<CreateRewardCategoryCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(
        CreateRewardCategoryCommand request,
        CancellationToken ct)
    {
        // Unique name check
        var nameExists = await context.RewardCategories
            .AnyAsync(x =>
                x.IsActive &&
                x.Name.ToLower() == request.Name.Trim().ToLower(),
                ct);

        if (nameExists)
            return Result<Guid>.Failure(
                new Error(
                    "reward_category.name_duplicate",
                    $"A reward category named \"{request.Name}\" already exists.",
                    409,
                    "reward-categories"));

        var rewardCategory = new RewardCategory
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Bytes = request.Bytes,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        context.RewardCategories.Add(rewardCategory);

        await context.SaveChangesAsync(ct);

        return Result<Guid>.Ok(rewardCategory.Id);
    }
}
