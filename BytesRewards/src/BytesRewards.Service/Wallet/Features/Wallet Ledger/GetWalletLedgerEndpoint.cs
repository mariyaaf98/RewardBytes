using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

namespace BytesRewards.Service.Wallets.Features.GetWalletLedger;

public sealed class GetWalletLedgerEndpoint(
    IMediator mediator)
    : SecureFastEndpoint<
        GetWalletLedgerRequest,
        List<GetWalletLedgerResponse>>
{
    public override void Configure()
    {
        Get("/wallets/ledger/{userId}");

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

    protected override async Task<Result<List<GetWalletLedgerResponse>>>
        ExecuteAsync(
            GetWalletLedgerRequest req,
            CancellationToken ct)
    {
        var ledger =
            await mediator.Send(
                new GetWalletLedgerQuery(
                    req.UserId),
                ct);

        return Result<List<GetWalletLedgerResponse>>
            .Ok(ledger);
    }
}