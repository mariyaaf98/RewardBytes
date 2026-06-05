using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.RewardCategories.Features.UpdateRewardCategory;

public sealed class UpdateRewardCategoryCommandHandler(
    ApplicationDbContext context)
    : ICommandHandler<
        UpdateRewardCategoryCommand,
        Result<bool>>
{
    public async ValueTask<Result<bool>> Handle(
        UpdateRewardCategoryCommand request,
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

        category.Name = request.Name;

        category.Description = request.Description;

        category.Bytes = request.Bytes;

        await context.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }
}