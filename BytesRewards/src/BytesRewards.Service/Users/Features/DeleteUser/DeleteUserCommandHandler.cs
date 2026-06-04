using Microsoft.EntityFrameworkCore;

using AppWeaver.Mediator.Interfaces;

using AppWeaver.Results;

using BytesRewards.Service.Infrastructure;
using BytesRewards.Service.Infrastructure.Security.Keycloak;

namespace BytesRewards.Service.Users.Features.DeleteUser;

public sealed class DeleteUserCommandHandler(
    ApplicationDbContext context,
    IKeycloakAdminService keycloakAdminService)
    : ICommandHandler<DeleteUserCommand, Result<bool>>
{
    public async ValueTask<Result<bool>> Handle(
        DeleteUserCommand request,
        CancellationToken ct)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                ct);

        if (user is null)
        {
            return Result<bool>.Ok(false);
        }

        var token =
    await keycloakAdminService
        .GetAdminTokenAsync(ct);

        await keycloakAdminService
            .DisableUserAsync(
                token,
                user.KeycloakUserId,
                ct);

        user.IsActive = false;



        user.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }
}