using Microsoft.EntityFrameworkCore;

using AppWeaver.Mediator.Interfaces;

using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.RewardCategories.Features.GetRewardCategories;

public sealed class GetRewardCategoriesQueryHandler(
    ApplicationDbContext context)
    : IQueryHandler<
        GetRewardCategoriesQuery,
        List<RewardCategoryResponse>>
{
    public async ValueTask<List<RewardCategoryResponse>> Handle(
        GetRewardCategoriesQuery request,
        CancellationToken ct)
    {
        return await context.RewardCategories
            .Select(x => new RewardCategoryResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Bytes = x.Bytes,
                IsActive = x.IsActive
            })
            .ToListAsync(ct);
    }
}