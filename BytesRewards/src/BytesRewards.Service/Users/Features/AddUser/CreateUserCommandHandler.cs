using AppWeaver.Mediator.Interfaces;
using AppWeaver.Repository.Abstractions;
using BytesRewards.Service.Infrastructure;
using AppWeaver.Results;
using BytesRewards.Service.Infrastructure.Security.Keycloak;

using BytesRewards.Service.Users.Domain;

namespace BytesRewards.Service.Features.Users.CreateUser;


public class CreateUserCommandHandler(
    ApplicationDbContext _context,
    IKeycloakAdminService _keycloakAdminService
)
    : ICommandHandler<CreateUserCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(
    CreateUserCommand request,
    CancellationToken ct)
    {
        var token =
            await _keycloakAdminService.GetAdminTokenAsync(ct);

        var keycloakUserId =
            await _keycloakAdminService.CreateUserAsync(
                token,
                request.FirstName,
                request.LastName,
                request.Email,
                request.TemporaryPassword,
                ct);


        await _keycloakAdminService.AssignRoleAsync(
            token,
            keycloakUserId,
            request.Role.Trim().ToLowerInvariant(),
            ct);


        var user = new User
        {
            Id = Guid.NewGuid(),

            EmployeeId =
                $"EMP-{Random.Shared.Next(1000, 9999)}",

            FirstName = request.FirstName,

            LastName = request.LastName,

            Email = request.Email,

            PhoneNumber = request.PhoneNumber,

            Designation = request.Designation,

            DepartmentId = request.DepartmentId,

            IsActive = true,

            CreatedAt = DateTime.UtcNow,

            ProfileImageUrl = string.Empty,

            KeycloakUserId = keycloakUserId
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync(ct);

        return Result<Guid>.Ok(user.Id);
    }
}