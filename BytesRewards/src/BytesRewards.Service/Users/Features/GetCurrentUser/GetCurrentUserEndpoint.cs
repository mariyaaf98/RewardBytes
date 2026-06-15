using FastEndpoints;

using AppWeaver.Mediator;

using System.Security.Claims;

namespace BytesRewards.Service.Users.Features.GetCurrentUser;

public sealed class GetCurrentUserEndpoint(
    IMediator mediator)
    : EndpointWithoutRequest<GetCurrentUserResponse>
{
    public override void Configure()
    {
        Get("/users/me");

        Roles(
            "employee",
            "manager",
            "admin");

        Options(option =>
            option.WithTags("01 - Users"));
    }

    public override async Task HandleAsync(
        CancellationToken ct)
    {
        var keycloakUserId =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(keycloakUserId))
        {
            ThrowError(
                "Keycloak user id not found in token.");
        }

        Response =
            await mediator.Send(
                new GetCurrentUserQuery(
                    keycloakUserId!),
                ct);
    }
}