using Microsoft.EntityFrameworkCore;

using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.RewardsCatalog.Features.UpdateRewardItem;

public sealed class UpdateRewardItemCommandHandler(
    ApplicationDbContext context)
    : ICommandHandler<
        UpdateRewardItemCommand,
        Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(
        UpdateRewardItemCommand request,
        CancellationToken ct)
    {
        var rewardItem =
            await context.RewardItems
                .FirstOrDefaultAsync(
                    x => x.Id == request.Id,
                    ct);

        if (rewardItem is null)
        {
            throw new Exception(
                "Reward item not found");
        }

        rewardItem.ProductCode =
            request.ProductCode;

        rewardItem.Name =
            request.Name;

        rewardItem.Description =
            request.Description;

        rewardItem.RequiredBytes =
            request.RequiredBytes;

        rewardItem.IsActive =
            request.IsActive;

        rewardItem.ImageUrl =
            request.ImageUrl;

        await context.SaveChangesAsync(ct);

        return Result<Guid>.Ok(
            rewardItem.Id);
    }
}