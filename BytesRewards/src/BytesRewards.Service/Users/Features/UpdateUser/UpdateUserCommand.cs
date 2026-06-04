using AppWeaver.Mediator.Interfaces;

using AppWeaver.Results;


namespace BytesRewards.Service.Users.Features.UpdateUser;


public sealed record UpdateUserCommand(

    Guid Id,

    string FirstName,

    string LastName,

    string PhoneNumber,

    string Designation,
    
    string Email,

    string Role,

    Guid DepartmentId

) : ICommand<Result<bool>>;