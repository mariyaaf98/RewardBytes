using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

namespace BytesRewards.Service.Rewards.Features.CreateReward;

public sealed class CreateRewardEndpoint(
    IMediator mediator)
    : SecureFastEndpoint<
        CreateRewardRequest,
        Guid>
{
    public override void Configure()
    {
        Post("/rewards");

        Roles("manager");

        Options(option =>
            option.WithTags("06 - Rewards"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy()
        => new()
        {
            SecurityLevel = SecurityLevel.Internal,
            CachePolicy = CachePolicy.NoStore
        };

    protected override async Task<Result<Guid>>
        ExecuteAsync(
            CreateRewardRequest req,
            CancellationToken ct)
    {
        return await mediator.Send(
            new CreateRewardCommand(
                req.FromUserId,
                req.ToUserId,
                req.RewardCategoryId,
                req.Reason),
            ct);
    }
}