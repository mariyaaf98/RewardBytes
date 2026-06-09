using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

using BytesRewards.Service.Infrastructure;
using BytesRewards.Service.Rewards.Domain;

namespace BytesRewards.Service.Rewards.Features.CreateReward;

public sealed class CreateRewardCommandHandler(
    ApplicationDbContext context)
    : ICommandHandler<
        CreateRewardCommand,
        Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(
        CreateRewardCommand request,
        CancellationToken ct)
    {
        var reward = new Reward
        {
            Id = Guid.NewGuid(),

            FromUserId = request.FromUserId,

            ToUserId = request.ToUserId,

            RewardCategoryId =
                request.RewardCategoryId,

            Reason = request.Reason,

            CreatedAt = DateTime.UtcNow
        };

        context.Rewards.Add(reward);

        await context.SaveChangesAsync(ct);

        return Result<Guid>.Ok(reward.Id);
    }
}