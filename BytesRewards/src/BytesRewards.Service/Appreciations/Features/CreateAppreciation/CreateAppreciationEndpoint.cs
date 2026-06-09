using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;
using Microsoft.EntityFrameworkCore;
using BytesRewards.Service.Infrastructure;
using System.Security.Claims;

namespace BytesRewards.Service.Appreciations.Features.CreateAppreciation;

public sealed class CreateAppreciationEndpoint(
    IMediator mediator, ApplicationDbContext context)
    : SecureFastEndpoint<CreateAppreciationRequest, Guid>
{
    public override void Configure()
    {
        Post("/appreciations");

        Roles(
            "employee",
            "manager",
            "admin");

        Options(option =>
            option.WithTags("04 - Appreciations"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy()
        => new()
        {
            SecurityLevel = SecurityLevel.Internal,
            CachePolicy = CachePolicy.NoStore
        };

    protected override async Task<Result<Guid>> ExecuteAsync(
    CreateAppreciationRequest req,
    CancellationToken ct)
    {
        var keycloakUserId =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(keycloakUserId))
        {
            throw new Exception("Keycloak user id not found");
        }

        var currentUser =
            await context.Users.FirstOrDefaultAsync(
                x => x.KeycloakUserId == keycloakUserId,
                ct);
                

        if (currentUser is null)
        {
            throw new Exception("User not found");
        }

        return await mediator.Send(
            new CreateAppreciationCommand(
                currentUser.Id,
                req.ToUserId,
                req.Message),
            ct);
    }


}