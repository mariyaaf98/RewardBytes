using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

using BytesRewards.Service.Departments.Domain;
using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.Departments.Features.CreateDepartment;

public sealed class CreateDepartmentCommandHandler(
    ApplicationDbContext context
)
    : ICommandHandler<CreateDepartmentCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(
        CreateDepartmentCommand request,
        CancellationToken ct)
    {
        var department = new Department
        {
            Id = Guid.NewGuid(),

            Name = request.Name,

            Description = request.Description,

            IsActive = true,

            CreatedAt = DateTime.UtcNow
        };

        context.Departments.Add(department);

        await context.SaveChangesAsync(ct);

        return Result<Guid>.Ok(department.Id);
    }
}