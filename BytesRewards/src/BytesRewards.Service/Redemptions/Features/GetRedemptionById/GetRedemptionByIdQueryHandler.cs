using Microsoft.EntityFrameworkCore;

using AppWeaver.Mediator.Interfaces;

using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.Redemptions.Features.GetRedemptionById;

public sealed class GetRedemptionByIdQueryHandler(
    ApplicationDbContext context)
    : IQueryHandler<
        GetRedemptionByIdQuery,
        GetRedemptionByIdResponse>
{
    public async ValueTask<GetRedemptionByIdResponse> Handle(
        GetRedemptionByIdQuery request,
        CancellationToken ct)
    {
        var redemption =
            await context.Redemptions
                .FirstOrDefaultAsync(
                    x => x.Id == request.Id,
                    ct);

        if (redemption is null)
        {
            throw new Exception(
                "Redemption not found");
        }

        return new GetRedemptionByIdResponse
        {
            RedemptionId = redemption.Id,

            ProductName   = redemption.ProductName,

            RedeemedBytes =
                redemption.RedeemedBytes,

            Status =
                redemption.Status,

            RedeemedAt =
                redemption.CreatedAt
        };
    }
}