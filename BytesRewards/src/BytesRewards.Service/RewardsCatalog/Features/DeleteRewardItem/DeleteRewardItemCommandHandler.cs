using Microsoft.EntityFrameworkCore;

using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.RewardsCatalog.Features.DeleteRewardItem;

public sealed class DeleteRewardItemCommandHandler(
    ApplicationDbContext context)
    : ICommandHandler<
        DeleteRewardItemCommand,
        Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(
        DeleteRewardItemCommand request,
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

        rewardItem.IsActive = false;

        await context.SaveChangesAsync(ct);


        return Result<Guid>.Ok(
            rewardItem.Id);
    }
}