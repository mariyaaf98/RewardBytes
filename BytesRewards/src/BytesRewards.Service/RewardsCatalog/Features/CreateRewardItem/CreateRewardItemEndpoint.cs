using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;

namespace BytesRewards.Service.RewardsCatalog.Features.CreateRewardItem;

public sealed class CreateRewardItemEndpoint(
    IMediator mediator)
    : SecureFastEndpoint<
        CreateRewardItemRequest,
        Guid>
{
    public override void Configure()
    {
        Post("/reward-items");

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
            CreateRewardItemRequest req,
            CancellationToken ct)
    {
        return await mediator.Send(
            new CreateRewardItemCommand(
                req.ProductCode,
                req.Name,
                req.Description,
                req.RequiredBytes,
                req.IsActive,
                req.ImageUrl),
            ct);
    }
}