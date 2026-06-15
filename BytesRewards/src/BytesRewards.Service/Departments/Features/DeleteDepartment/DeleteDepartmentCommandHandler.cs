using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;
using BytesRewards.Service.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BytesRewards.Service.Departments.Features.DeleteDepartment;

public sealed class DeleteDepartmentCommandHandler(
    ApplicationDbContext context
) : ICommandHandler<DeleteDepartmentCommand, Result<bool>>
{
    public async ValueTask<Result<bool>> Handle(
        DeleteDepartmentCommand request,
        CancellationToken ct)
    {
        var department = await context.Departments
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (department is null)
            return Result<bool>.Ok(false);

        // Soft delete — mark inactive instead of removing the record
        department.IsActive = false;
        department.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }
}
