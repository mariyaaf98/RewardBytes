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
                .Include(x => x.Designation)
                .Include(x => x.Department)
                .OrderBy(x => x.FirstName)
                .Select(x => new UserLookupResponse
                {
                    Id              = x.Id,
                    FullName        = x.FirstName + " " + x.LastName,
                    DesignationName = x.Designation != null ? x.Designation.Name : string.Empty,
                    DepartmentName  = x.Department != null ? x.Department.Name  : string.Empty
                })
                .ToListAsync(ct);

        return Result<List<UserLookupResponse>>
            .Ok(users);
    }
}