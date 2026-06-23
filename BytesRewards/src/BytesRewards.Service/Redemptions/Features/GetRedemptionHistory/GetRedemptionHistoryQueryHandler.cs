using Microsoft.EntityFrameworkCore;

using AppWeaver.Mediator.Interfaces;

using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.Redemptions.Features.GetRedemptionHistory;

public sealed class GetRedemptionHistoryQueryHandler(
    ApplicationDbContext context)
    : IQueryHandler<
        GetRedemptionHistoryQuery,
        List<GetRedemptionHistoryResponse>>
{
    public async ValueTask<List<GetRedemptionHistoryResponse>> Handle(
        GetRedemptionHistoryQuery request,
        CancellationToken ct)
    {
        return await context.Redemptions
            .Where(x =>
                x.UserId == request.UserId)
            .Select(x => new GetRedemptionHistoryResponse
            {
                RedemptionId = x.Id,

                ProductName = x.ProductName,

                RedeemedBytes =
                    x.RedeemedBytes,

                Status =
                    x.Status,

                RedeemedAt =
                    x.CreatedAt
            })
            .ToListAsync(ct);
    }
}