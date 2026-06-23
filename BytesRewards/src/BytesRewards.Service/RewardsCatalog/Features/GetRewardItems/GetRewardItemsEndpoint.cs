using FastEndpoints;

using AppWeaver.Mediator;

namespace BytesRewards.Service.RewardsCatalog.Features.GetRewardItems;

public sealed class GetRewardItemsEndpoint(
    IMediator mediator)
    : EndpointWithoutRequest<
        List<RewardItemResponse>>
{
    public override void Configure()
    {
        Get("/reward-items");

        Roles(
            "employee",
            "manager",
            "admin");

        Options(option =>
            option.WithTags(
                "09 - Rewards Catalog"));
    }

    public override async Task HandleAsync(
        CancellationToken ct)
    {
        Response =
            await mediator.Send(
                new GetRewardItemsQuery(),
                ct);
    }
}