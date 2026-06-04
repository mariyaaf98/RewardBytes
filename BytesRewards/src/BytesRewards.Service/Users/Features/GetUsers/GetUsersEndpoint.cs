using AppWeaver.FastEndpoint;

using AppWeaver.Mediator;

using AppWeaver.Results;

namespace BytesRewards.Service.Users.Features.GetUsers;

public sealed class GetUsersEndpoint(IMediator mediator)
    : SecureFastEndpoint<GetUsersRequest, List<UserResponse>>
{
    public override void Configure()
    {
        Get("/users");

        Roles("admin");

        Summary(summary =>
        {
            summary.Summary = "Get all users";
        });

        Options(option => option.WithTags("01 - Users"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy() => new()
    {
        SecurityLevel = SecurityLevel.Internal,
        CachePolicy = CachePolicy.NoStore
    };

    protected override async Task<Result<List<UserResponse>>> ExecuteAsync(
        GetUsersRequest req,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new GetUsersQuery(),
            ct);

       return result;
    }
}

public sealed class GetUsersRequest
{
    public string? Search { get; set; }
}