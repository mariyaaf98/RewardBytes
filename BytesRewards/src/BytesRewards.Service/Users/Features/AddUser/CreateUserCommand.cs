using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.Features.Users.CreateUser;

public record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Designation,
    string TemporaryPassword,
    string Role,
    Guid DepartmentId
) : ICommand<Result<Guid>>;