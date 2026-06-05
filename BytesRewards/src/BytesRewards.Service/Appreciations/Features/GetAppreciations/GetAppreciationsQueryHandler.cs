using Microsoft.EntityFrameworkCore;

using AppWeaver.Mediator.Interfaces;

using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.Appreciations.Features.GetAppreciations;

public sealed class GetAppreciationsQueryHandler(
    ApplicationDbContext context)
    : IQueryHandler<
        GetAppreciationsQuery,
        List<AppreciationResponse>>
{
    public async ValueTask<List<AppreciationResponse>> Handle(
        GetAppreciationsQuery request,
        CancellationToken ct)
    {
        return await context.Appreciations
            .Select(x => new AppreciationResponse
            {
                Id = x.Id,

                FromUserName =
                    context.Users
                        .Where(u => u.Id == x.FromUserId)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault() ?? string.Empty,

                ToUserName =
                    context.Users
                        .Where(u => u.Id == x.ToUserId)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault() ?? string.Empty,

                Message = x.Message,

                CreatedAt = x.CreatedAt
            })
            .ToListAsync(ct);
    }
}