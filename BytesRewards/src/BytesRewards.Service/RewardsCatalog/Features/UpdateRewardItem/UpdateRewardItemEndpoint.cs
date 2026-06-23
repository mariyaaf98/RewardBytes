using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

namespace BytesRewards.Service.RewardsCatalog.Features.UpdateRewardItem;

public sealed class UpdateRewardItemEndpoint(
    IMediator mediator)
    : SecureFastEndpoint<
        UpdateRewardItemRequest,
        Guid>
{
    public override void Configure()
    {
        Put("/reward-items/{id}");

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
            UpdateRewardItemRequest req,
            CancellationToken ct)
    {
        return await mediator.Send(
            new UpdateRewardItemCommand(
                req.Id,
                req.ProductCode,
                req.Name,
                req.Description,
                req.RequiredBytes,
                req.IsActive,
                req.ImageUrl),
            ct);
    }
}