using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;
using BytesRewards.Service.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BytesRewards.Service.Users.Features.UpdateProfileImage;

public sealed class UpdateProfileImageCommandHandler(
    ApplicationDbContext context)
    : ICommandHandler<UpdateProfileImageCommand, Result<bool>>
{
    public async ValueTask<Result<bool>> Handle(
        UpdateProfileImageCommand request,
        CancellationToken ct)
    {
        var user =
            await context.Users
                .FirstOrDefaultAsync(
                    x => x.KeycloakUserId == request.KeycloakUserId,
                    ct);

        if (user is null)
            throw new Exception("User not found.");

        user.ProfileImageUrl = request.ProfileImageUrl;

        await context.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }
}
