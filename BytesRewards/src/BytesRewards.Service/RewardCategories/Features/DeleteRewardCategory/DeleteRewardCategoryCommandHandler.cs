using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.RewardCategories.Features.DeleteRewardCategory;

public sealed class DeleteRewardCategoryCommandHandler(
    ApplicationDbContext context)
    : ICommandHandler<
        DeleteRewardCategoryCommand,
        Result<bool>>
{
    public async ValueTask<Result<bool>> Handle(
        DeleteRewardCategoryCommand request,
        CancellationToken ct)
    {
        var category =
            await context.RewardCategories.FindAsync(
                [request.Id],
                ct);

        if (category is null)
        {
            throw new Exception(
                "Reward Category not found");
        }

        category.IsActive = false;

        await context.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }
}