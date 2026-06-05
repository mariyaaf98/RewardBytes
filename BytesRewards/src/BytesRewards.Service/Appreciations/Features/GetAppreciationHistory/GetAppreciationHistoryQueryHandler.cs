using Microsoft.EntityFrameworkCore;

using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

using BytesRewards.Service.Infrastructure;
using BytesRewards.Service.Appreciations.Features.GetAppreciations;

namespace BytesRewards.Service.Appreciations.Features.GetAppreciationHistory;

public sealed class GetAppreciationHistoryQueryHandler(
    ApplicationDbContext context)
    : IQueryHandler<
        GetAppreciationHistoryQuery,
        Result<List<AppreciationResponse>>>
{
    public async ValueTask<Result<List<AppreciationResponse>>> Handle(
        GetAppreciationHistoryQuery request,
        CancellationToken ct)
    {
        var appreciations =
            await context.Appreciations
                .Where(x =>
                    x.ToUserId == request.UserId)
                .OrderByDescending(x =>
                    x.CreatedAt)
                .Select(x => new AppreciationResponse
                {
                    Id = x.Id,

                    FromUserName =
                        context.Users
                            .Where(u => u.Id == x.FromUserId)
                            .Select(u =>
                                u.FirstName + " " + u.LastName)
                            .FirstOrDefault() ?? string.Empty,

                    ToUserName =
                        context.Users
                            .Where(u => u.Id == x.ToUserId)
                            .Select(u =>
                                u.FirstName + " " + u.LastName)
                            .FirstOrDefault() ?? string.Empty,

                    Message = x.Message,

                    CreatedAt = x.CreatedAt
                })
                .ToListAsync(ct);

        return Result<List<AppreciationResponse>>
            .Ok(appreciations);
    }
}