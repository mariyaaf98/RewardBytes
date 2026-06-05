using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

using BytesRewards.Service.Infrastructure;
using BytesRewards.Service.RewardCategories.Domain;

namespace BytesRewards.Service.RewardCategories.Features.CreateRewardCategory;

public sealed class CreateRewardCategoryCommandHandler(
    ApplicationDbContext context)
    : ICommandHandler<
        CreateRewardCategoryCommand,
        Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(
        CreateRewardCategoryCommand request,
        CancellationToken ct)
    {
        var rewardCategory =
            new RewardCategory
            {
                Id = Guid.NewGuid(),

                Name = request.Name,

                Description = request.Description,

                Bytes = request.Bytes,

                IsActive = true,

                CreatedAt = DateTime.UtcNow
            };

        context.RewardCategories.Add(
            rewardCategory);

        await context.SaveChangesAsync(ct);

        return Result<Guid>.Ok(
            rewardCategory.Id);
    }
}