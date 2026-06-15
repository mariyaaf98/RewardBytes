using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.Departments.Features.DeleteDepartment;

public sealed record DeleteDepartmentCommand(
    Guid Id
) : ICommand<Result<bool>>;
