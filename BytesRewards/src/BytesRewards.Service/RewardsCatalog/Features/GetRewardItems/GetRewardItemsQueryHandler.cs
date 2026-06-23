using Microsoft.EntityFrameworkCore;

using AppWeaver.Mediator.Interfaces;

using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.RewardsCatalog.Features.GetRewardItems;

public sealed class GetRewardItemsQueryHandler(
    ApplicationDbContext context)
    : IQueryHandler<
        GetRewardItemsQuery,
        List<RewardItemResponse>>
{
    public async ValueTask<List<RewardItemResponse>> Handle(
        GetRewardItemsQuery request,
        CancellationToken ct)
    {
        return await context.RewardItems
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new RewardItemResponse
            {
                Id = x.Id,
                ProductCode = x.ProductCode,
                Name = x.Name,
                Description = x.Description,
                RequiredBytes = x.RequiredBytes,
                IsActive = x.IsActive,
                ImageUrl = x.ImageUrl
            })
            .ToListAsync(ct);
    }
}