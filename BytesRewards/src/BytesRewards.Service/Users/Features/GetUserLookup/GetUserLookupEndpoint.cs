using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;

namespace BytesRewards.Service.Users.Features.GetUserLookup;

public sealed class GetUserLookupEndpoint(
    IMediator mediator)
    : SecureFastEndpoint<
        GetUserLookupRequest,
        List<UserLookupResponse>>
{
    public override void Configure()
    {
        Get("/users/lookup");

        Roles(
            "employee",
            "manager",
            "admin");

        Summary(summary =>
        {
            summary.Summary =
                "Get users for dropdown";
        });

        Options(option =>
            option.WithTags("01 - Users"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy()
        => new()
        {
            SecurityLevel = SecurityLevel.Internal,
            CachePolicy = CachePolicy.NoStore
        };

    protected override async Task<Result<List<UserLookupResponse>>> ExecuteAsync(
        GetUserLookupRequest req,
        CancellationToken ct)
    {
        var result =
            await mediator.Send(
                new GetUserLookupQuery(),
                ct);

        return result;
    }
}