using Microsoft.EntityFrameworkCore;
using AppWeaver.Mediator.Interfaces;
using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.Appreciations.Features.GetAppreciations;

public sealed class GetAppreciationsQueryHandler(
    ApplicationDbContext context)
    : IQueryHandler<GetAppreciationsQuery, List<AppreciationResponse>>
{
    public async ValueTask<List<AppreciationResponse>> Handle(
        GetAppreciationsQuery request,
        CancellationToken ct)
    {
        return await context.Appreciations
            .Select(x => new AppreciationResponse
            {
                Id           = x.Id,
                FromUserId   = x.FromUserId,
                ToUserId     = x.ToUserId,
                FromUserName = x.FromUserName,
                ToUserName   = x.ToUserName,
                Message      = x.Message,
                CreatedAt    = x.CreatedAt
            })
            .ToListAsync(ct);
    }
}
