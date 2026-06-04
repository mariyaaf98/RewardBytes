using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.Departments.Features.CreateDepartment;

public sealed record CreateDepartmentCommand(
    string Name,
    string Description
) : ICommand<Result<Guid>>;