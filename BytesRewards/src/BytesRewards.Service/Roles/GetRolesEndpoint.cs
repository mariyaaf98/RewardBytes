using FastEndpoints;
using AppWeaver.Mediator;

namespace BytesRewards.Service.Roles.Features.GetRoles;

public sealed class GetRolesEndpoint(
    IMediator mediator)
    : EndpointWithoutRequest<List<RoleResponse>>
{
    public override void Configure()
    {
        Get("/roles");

        AllowAnonymous();

        Options(option =>
            option.WithTags("03 - Roles"));
    }

    public override async Task HandleAsync(
        CancellationToken ct)
    {
        Response =
            await mediator.Send(
                new GetRolesQuery(),
                ct);
    }
}