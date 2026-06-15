using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

namespace BytesRewards.Service.Wallets.Features.GetWallet;

public sealed class GetWalletEndpoint(
    IMediator mediator)
    : SecureFastEndpoint<
        GetWalletRequest,
        GetWalletResponse>
{
    public override void Configure()
    {
        Get("/wallets/{userId}");

        Roles(
            "employee",
            "manager",
            "admin");

        Options(option =>
            option.WithTags("07 - Wallets"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy()
        => new()
        {
            SecurityLevel = SecurityLevel.Internal,
            CachePolicy = CachePolicy.NoStore
        };

    protected override async Task<Result<GetWalletResponse>>
        ExecuteAsync(
            GetWalletRequest req,
            CancellationToken ct)
    {
        var wallet =
            await mediator.Send(
                new GetWalletQuery(
                    req.UserId),
                ct);

        return Result<GetWalletResponse>
            .Ok(wallet);
    }
}