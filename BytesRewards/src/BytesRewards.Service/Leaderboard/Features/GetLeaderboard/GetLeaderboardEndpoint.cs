using FastEndpoints;

using AppWeaver.Mediator;

namespace BytesRewards.Service.Leaderboard.Features.GetLeaderboard;

public sealed class GetLeaderboardEndpoint(
    IMediator mediator)
    : EndpointWithoutRequest<
        List<GetLeaderboardResponse>>
{
    public override void Configure()
    {
        Get("/leaderboard");

        Roles(
            "employee",
            "manager",
            "admin");

        Options(option =>
            option.WithTags("08 - Leaderboard"));
    }

    public override async Task HandleAsync(
        CancellationToken ct)
    {
        Response =
            await mediator.Send(
                new GetLeaderboardQuery(),
                ct);
    }
}