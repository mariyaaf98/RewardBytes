using Microsoft.EntityFrameworkCore;

using AppWeaver.Mediator.Interfaces;

using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.RewardsCatalog.Features.GetRewardItemById;

public sealed class GetRewardItemByIdQueryHandler(
    ApplicationDbContext context)
    : IQueryHandler<
        GetRewardItemByIdQuery,
        GetRewardItemByIdResponse>
{
    public async ValueTask<GetRewardItemByIdResponse> Handle(
        GetRewardItemByIdQuery request,
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

        return new GetRewardItemByIdResponse
        {
            Id = rewardItem.Id,
            ProductCode = rewardItem.ProductCode,
            Name = rewardItem.Name,
            Description = rewardItem.Description,
            RequiredBytes = rewardItem.RequiredBytes,
            IsActive = rewardItem.IsActive,
            ImageUrl = rewardItem.ImageUrl
        };
    }
}