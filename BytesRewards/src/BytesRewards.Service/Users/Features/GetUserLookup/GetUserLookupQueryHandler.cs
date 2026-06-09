using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

using BytesRewards.Service.Infrastructure;

using Microsoft.EntityFrameworkCore;

namespace BytesRewards.Service.Users.Features.GetUserLookup;

public sealed class GetUserLookupQueryHandler(
    ApplicationDbContext context)
    : IQueryHandler<
        GetUserLookupQuery,
        Result<List<UserLookupResponse>>>
{
    public async ValueTask<
        Result<List<UserLookupResponse>>> Handle(
        GetUserLookupQuery request,
        CancellationToken ct)
    {
        var users =
            await context.Users
                .Where(x => x.IsActive)
                .OrderBy(x => x.FirstName)
                .Select(x => new UserLookupResponse
                {
                    Id = x.Id,

                    FullName =
                        x.FirstName + " " + x.LastName
                })
                .ToListAsync(ct);

        return Result<List<UserLookupResponse>>
            .Ok(users);
    }
}