using Microsoft.EntityFrameworkCore;
using AppWeaver.Mediator.Interfaces;
using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.Designations.Features.GetDesignations;

public sealed class GetDesignationsQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetDesignationsQuery, List<DesignationResponse>>
{
    public async ValueTask<List<DesignationResponse>> Handle(
        GetDesignationsQuery request, CancellationToken ct)
    {
        return await context.Designations
            .OrderBy(x => x.Name)
            .Select(x => new DesignationResponse
            {
                Id          = x.Id,
                Name        = x.Name,
                Description = x.Description,
                IsActive    = x.IsActive
            })
            .ToListAsync(ct);
    }
}
