using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.Departments.Features.UpdateDepartment;

public sealed record UpdateDepartmentCommand(
    Guid Id,
    string Name,
    string Description
) : ICommand<Result<bool>>;
