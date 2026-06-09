using FastEndpoints;
using AppWeaver.Mediator;

namespace BytesRewards.Service.Rewards.Features.GetRewards;

public sealed class GetRewardsEndpoint(
    IMediator mediator)
    : EndpointWithoutRequest<List<RewardResponse>>
{
    public override void Configure()
    {
        Get("/rewards");

        Roles(
            "manager",
            "admin");

        Options(option =>
            option.WithTags("06 - Rewards"));
    }

    public override async Task HandleAsync(
        CancellationToken ct)
    {
        Response =
            await mediator.Send(
                new GetRewardsQuery(),
                ct);
    }
}