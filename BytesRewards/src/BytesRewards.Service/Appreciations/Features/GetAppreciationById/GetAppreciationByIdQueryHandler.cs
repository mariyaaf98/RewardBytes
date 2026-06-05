using Microsoft.EntityFrameworkCore;

using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

using BytesRewards.Service.Infrastructure;
using BytesRewards.Service.Appreciations.Features.GetAppreciations;

namespace BytesRewards.Service.Appreciations.Features.GetAppreciationById;

public sealed class GetAppreciationByIdQueryHandler(
    ApplicationDbContext context)
    : IQueryHandler<
        GetAppreciationByIdQuery,
        Result<AppreciationResponse>>
{
    public async ValueTask<Result<AppreciationResponse>> Handle(
        GetAppreciationByIdQuery request,
        CancellationToken ct)
    {
        var appreciation =
            await context.Appreciations
                .FirstOrDefaultAsync(
                    x => x.Id == request.Id,
                    ct);

        if (appreciation is null)
        {
            return Result<AppreciationResponse>.Failure(
                new Error(
                    "appreciation.not_found",
                    "Appreciation not found",
                    404,
                    "appreciations"));
        }

        var response =
            new AppreciationResponse
            {
                Id = appreciation.Id,

                FromUserName =
                    context.Users
                        .Where(x => x.Id == appreciation.FromUserId)
                        .Select(x => x.FirstName + " " + x.LastName)
                        .FirstOrDefault() ?? string.Empty,

                ToUserName =
                    context.Users
                        .Where(x => x.Id == appreciation.ToUserId)
                        .Select(x => x.FirstName + " " + x.LastName)
                        .FirstOrDefault() ?? string.Empty,

                Message = appreciation.Message,

                CreatedAt = appreciation.CreatedAt
            };

        return Result<AppreciationResponse>.Ok(response);
    }
}