using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

namespace BytesRewards.Service.RewardsCatalog.Features.DeleteRewardItem;

public sealed class DeleteRewardItemEndpoint(
    IMediator mediator)
    : SecureFastEndpoint<
        DeleteRewardItemRequest,
        Guid>
{
    public override void Configure()
    {
        Delete("/reward-items/{id}");

        Roles("admin");

        Options(option =>
            option.WithTags(
                "09 - Rewards Catalog"));
    }

    protected override SecurityCachePolicy
        GetSecurityCachePolicy()
        => new()
        {
            SecurityLevel =
                SecurityLevel.Internal,

            CachePolicy =
                CachePolicy.NoStore
        };

    protected override async Task<Result<Guid>>
        ExecuteAsync(
            DeleteRewardItemRequest req,
            CancellationToken ct)
    {
        return await mediator.Send(
            new DeleteRewardItemCommand(
                req.Id),
            ct);
    }
}