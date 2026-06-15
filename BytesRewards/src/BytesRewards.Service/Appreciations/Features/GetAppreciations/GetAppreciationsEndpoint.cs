using FastEndpoints;
using AppWeaver.Mediator;

namespace BytesRewards.Service.Appreciations.Features.GetAppreciations;

public sealed class GetAppreciationsEndpoint(
    IMediator mediator)
    : EndpointWithoutRequest<List<AppreciationResponse>>
{
    public override void Configure()
    {
        Get("/appreciations");

        Roles(
            "employee",
            "manager",
            "admin");

        Options(option =>
            option.WithTags("04 - Appreciations"));
    }

    public override async Task HandleAsync(
        CancellationToken ct)
    {
        Response = await mediator.Send(
            new GetAppreciationsQuery(),
            ct);
    }
}
