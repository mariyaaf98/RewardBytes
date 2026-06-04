using AppWeaver.FastEndpoint;

using AppWeaver.Mediator;

using AppWeaver.Results;

using AppWeaver.Web.Security;

namespace BytesRewards.Service.Users.Features.DeleteUser;

public sealed class DeleteUserEndpoint(
    IMediator mediator)
    : SecureFastEndpoint<DeleteUserRequest, bool>
{
    public override void Configure()
    {
        Delete("/users/{id}");

        Roles("admin");

        Options(option =>
            option.WithTags("01 - Users"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy() => new()
    {
        SecurityLevel = SecurityLevel.Internal,
        CachePolicy = CachePolicy.NoStore
    };

    protected override async Task<Result<bool>> ExecuteAsync(
        DeleteUserRequest req,
        CancellationToken ct)
    {
        return await mediator.Send(
            new DeleteUserCommand(req.Id),
            ct);
    }
}