using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

namespace BytesRewards.Service.Redemptions.Features.UpdateRedemptionStatus;

public sealed class UpdateRedemptionStatusEndpoint(
    IMediator mediator)
    : SecureFastEndpoint<
        UpdateRedemptionStatusRequest,
        Guid>
{
    public override void Configure()
    {
        Put("/redemptions/status");

        Roles("admin");

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

    protected override async Task<Result<Guid>>
        ExecuteAsync(
            UpdateRedemptionStatusRequest req,
            CancellationToken ct)
    {
        return await mediator.Send(
            new UpdateRedemptionStatusCommand(
                req.RedemptionId,
                req.Status),
            ct);
    }
}