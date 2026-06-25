using FastEndpoints;
using AppWeaver.Mediator;

namespace BytesRewards.Service.Rewards.Features.GetEmployeeRewardStatus;

public sealed class GetEmployeeRewardStatusEndpoint(IMediator mediator)
    : EndpointWithoutRequest<EmployeeRewardStatusResponse>
{
    public override void Configure()
    {
        Get("/rewards/employee-status");
        Roles("manager", "admin");
        Options(o => o.WithTags("06 - Rewards"));
    }

    public override async Task HandleAsync(CancellationToken ct)
        => Response = await mediator.Send(new GetEmployeeRewardStatusQuery(), ct);
}
