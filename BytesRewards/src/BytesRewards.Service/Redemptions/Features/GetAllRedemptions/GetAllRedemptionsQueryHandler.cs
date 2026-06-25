using Microsoft.EntityFrameworkCore;
using AppWeaver.Mediator.Interfaces;
using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.Redemptions.Features.GetAllRedemptions;

public sealed class GetAllRedemptionsQueryHandler(
    ApplicationDbContext context)
    : IQueryHandler<GetAllRedemptionsQuery, List<GetAllRedemptionsResponse>>
{
    public async ValueTask<List<GetAllRedemptionsResponse>> Handle(
        GetAllRedemptionsQuery request,
        CancellationToken ct)
    {
        return await context.Redemptions
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new GetAllRedemptionsResponse
            {
                RedemptionId  = x.Id,
                UserId        = x.UserId,
                UserName      = context.Users
                    .Where(u => u.Id == x.UserId)
                    .Select(u => u.FirstName + " " + u.LastName)
                    .FirstOrDefault() ?? string.Empty,
                ProductName   = x.ProductName,
                RedeemedBytes = x.RedeemedBytes,
                Status        = x.Status,
                RedeemedAt    = x.CreatedAt
            })
            .ToListAsync(ct);
    }
}
