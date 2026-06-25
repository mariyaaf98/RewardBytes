using FastEndpoints;
using AppWeaver.Mediator;

namespace BytesRewards.Service.Redemptions.Features.GetAllRedemptions;

/// <summary>
/// GET /redemptions — admin only.
/// Returns all redemptions across all users, newest first.
/// </summary>
public sealed class GetAllRedemptionsEndpoint(IMediator mediator)
    : EndpointWithoutRequest<List<GetAllRedemptionsResponse>>
{
    public override void Configure()
    {
        Get("/redemptions");
        Roles("admin");
        Options(o => o.WithTags("10 - Redemptions"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        Response = await mediator.Send(new GetAllRedemptionsQuery(), ct);
    }
}
