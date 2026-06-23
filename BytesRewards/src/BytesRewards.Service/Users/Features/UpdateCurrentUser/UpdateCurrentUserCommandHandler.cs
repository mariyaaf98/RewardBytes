using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;
using BytesRewards.Service.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BytesRewards.Service.Users.Features.UpdateCurrentUser;

public sealed class UpdateCurrentUserCommandHandler(
    ApplicationDbContext context)
    : ICommandHandler<UpdateCurrentUserCommand, Result<bool>>
{
    public async ValueTask<Result<bool>> Handle(
        UpdateCurrentUserCommand request,
        CancellationToken ct)
    {
        var user =
            await context.Users
                .FirstOrDefaultAsync(
                    x => x.KeycloakUserId == request.KeycloakUserId,
                    ct);

        if (user is null)
            throw new Exception("User not found.");

        user.FirstName   = request.FirstName;
        user.LastName    = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        user.UpdatedAt   = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }
}
