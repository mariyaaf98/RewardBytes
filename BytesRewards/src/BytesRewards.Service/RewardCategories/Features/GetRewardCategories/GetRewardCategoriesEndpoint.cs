using FastEndpoints;
using AppWeaver.Mediator;

namespace BytesRewards.Service.RewardCategories.Features.GetRewardCategories;

public sealed class GetRewardCategoriesEndpoint(
    IMediator mediator)
    : EndpointWithoutRequest<List<RewardCategoryResponse>>
{
    public override void Configure()
    {
        Get("/reward-categories");

        Roles(
            "manager",
            "admin");

        Options(option =>
            option.WithTags("05 - Reward Categories"));
    }

    public override async Task HandleAsync(
        CancellationToken ct)
    {
        Response =
            await mediator.Send(
                new GetRewardCategoriesQuery(),
                ct);
    }
}