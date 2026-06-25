using FastEndpoints;
using AppWeaver.Mediator;

namespace BytesRewards.Service.Designations.Features.GetDesignations;

public sealed class GetDesignationsEndpoint(IMediator mediator)
    : EndpointWithoutRequest<List<DesignationResponse>>
{
    public override void Configure()
    {
        Get("/designations");
        Roles("admin", "manager", "employee");
        Options(o => o.WithTags("12 - Designations"));
    }

    public override async Task HandleAsync(CancellationToken ct)
        => Response = await mediator.Send(new GetDesignationsQuery(), ct);
}
