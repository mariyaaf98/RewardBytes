using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;
using BytesRewards.Service.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BytesRewards.Service.Departments.Features.UpdateDepartment;

public sealed class UpdateDepartmentCommandHandler(
    ApplicationDbContext context
) : ICommandHandler<UpdateDepartmentCommand, Result<bool>>
{
    public async ValueTask<Result<bool>> Handle(
        UpdateDepartmentCommand request,
        CancellationToken ct)
    {
        var department = await context.Departments
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (department is null)
            return Result<bool>.Ok(false);

        department.Name = request.Name;
        department.Description = request.Description;
        department.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }
}
