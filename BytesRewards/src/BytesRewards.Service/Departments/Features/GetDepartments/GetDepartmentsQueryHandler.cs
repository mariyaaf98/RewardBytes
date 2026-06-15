using Microsoft.EntityFrameworkCore;

using AppWeaver.Mediator.Interfaces;

using BytesRewards.Service.Infrastructure;
using System.Linq;

namespace BytesRewards.Service.Departments.Features.GetDepartments;

public sealed class GetDepartmentsQueryHandler(
    ApplicationDbContext context
)
    : IQueryHandler<
        GetDepartmentsQuery,
        List<DepartmentResponse>
    >
{
    public async ValueTask<List<DepartmentResponse>> Handle(
        GetDepartmentsQuery request,
        CancellationToken ct
    )
    {
        return await context.Departments
            .Select(department => new DepartmentResponse
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description,
                IsActive = department.IsActive
            })
            .ToListAsync(ct);
    }
}