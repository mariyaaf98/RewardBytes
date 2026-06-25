using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.Features.Users.CreateUser;

public record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    Guid DesignationId,
    string TemporaryPassword,
    string Role,
    Guid DepartmentId
) : ICommand<Result<Guid>>;