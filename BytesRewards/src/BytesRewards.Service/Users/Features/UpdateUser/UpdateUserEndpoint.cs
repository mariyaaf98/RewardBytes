using AppWeaver.FastEndpoint;

using AppWeaver.Mediator;

using AppWeaver.Results;

using AppWeaver.Web.Security;


namespace BytesRewards.Service.Users.Features.UpdateUser;


public sealed class UpdateUserEndpoint(
    IMediator mediator
)
: SecureFastEndpoint<
    UpdateUserRequest,
    bool>
{

    public override void Configure()
    {

        Put("/users/{id}");

        Roles("admin");


        Summary(summary =>
        {
            summary.Summary =
                "Update user";
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



    protected override async Task<Result<bool>> ExecuteAsync(
        UpdateUserRequest req,
        CancellationToken ct)
    {

        var result = await mediator.Send(

            new UpdateUserCommand(
                req.Id,
                req.FirstName,
                req.LastName,
                req.PhoneNumber,
                req.Designation,
                req.Email,
                req.Role,
                req.DepartmentId
            ),
            ct);

        return result;

    }

}