using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

namespace BytesRewards.Service.Redemptions.Features.GetRedemptionById;

public sealed class GetRedemptionByIdEndpoint(
    IMediator mediator)
    : SecureFastEndpoint<
        GetRedemptionByIdRequest,
        GetRedemptionByIdResponse>
{
    public override void Configure()
    {
        Get("/redemptions/{id}");

        Roles(
            "employee",
            "manager",
            "admin");

        Options(option =>
            option.WithTags(
                "10 - Redemptions"));
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

    protected override async Task<Result<GetRedemptionByIdResponse>>
        ExecuteAsync(
            GetRedemptionByIdRequest req,
            CancellationToken ct)
    {
        var result =
            await mediator.Send(
                new GetRedemptionByIdQuery(
                    req.Id),
                ct);

        return Result<GetRedemptionByIdResponse>
            .Ok(result);
    }
}