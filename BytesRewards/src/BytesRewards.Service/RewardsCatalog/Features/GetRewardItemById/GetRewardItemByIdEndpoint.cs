using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

namespace BytesRewards.Service.RewardsCatalog.Features.GetRewardItemById;

public sealed class GetRewardItemByIdEndpoint(
    IMediator mediator)
    : SecureFastEndpoint<
        GetRewardItemByIdRequest,
        GetRewardItemByIdResponse>
{
    public override void Configure()
    {
        Get("/reward-items/{id}");

        Roles(
            "employee",
            "manager",
            "admin");

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

    protected override async Task<Result<GetRewardItemByIdResponse>>
        ExecuteAsync(
            GetRewardItemByIdRequest req,
            CancellationToken ct)
    {
        var result =
            await mediator.Send(
                new GetRewardItemByIdQuery(
                    req.Id),
                ct);

        return Result<GetRewardItemByIdResponse>
            .Ok(result);
    }
}