using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

using BytesRewards.Service.Infrastructure;

using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

namespace BytesRewards.Service.Rewards.Features.CreateReward;

public sealed class CreateRewardEndpoint(
    IMediator mediator,
    ApplicationDbContext context)
    : SecureFastEndpoint<CreateRewardRequest, Guid>
{
    public override void Configure()
    {
        Post("/rewards");

        Roles("manager");

        Options(option =>
            option.WithTags("06 - Rewards"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy()
        => new()
        {
            SecurityLevel = SecurityLevel.Internal,
            CachePolicy = CachePolicy.NoStore
        };

    protected override async Task<Result<Guid>> ExecuteAsync(
        CreateRewardRequest req,
        CancellationToken ct)
    {
        // Resolve manager's user ID from JWT (same pattern as CreateAppreciation)
        var keycloakUserId =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(keycloakUserId))
            throw new Exception("Keycloak user ID not found in token.");

        var currentUser = await context.Users
            .FirstOrDefaultAsync(x => x.KeycloakUserId == keycloakUserId, ct);

        if (currentUser is null)
            throw new Exception("Manager user not found.");

        return await mediator.Send(
            new CreateRewardCommand(
                currentUser.Id,
                req.ToUserId,
                req.RewardCategoryId,
                req.Reason),
            ct);
    }
}
