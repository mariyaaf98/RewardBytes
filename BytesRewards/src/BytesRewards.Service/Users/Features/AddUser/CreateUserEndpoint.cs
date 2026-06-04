using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;


namespace BytesRewards.Service.Features.Users.CreateUser;

public class CreateUserEndpoint(IMediator _mediator)
    : SecureFastEndpoint<CreateUserRequest, Guid>
{
    public override void Configure()
    {
        Post("/users");

        // AllowAnonymous();
        Roles("admin");

        Summary(s =>
        {
            s.Summary = "Create a new user";
        });

        Options(o => o.WithTags("01 - Users"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy() => new()
    {
        SecurityLevel = SecurityLevel.Internal,
        CachePolicy = CachePolicy.NoStore
    };

    protected override async Task<Result<Guid>> ExecuteAsync(
        CreateUserRequest req,
        CancellationToken ct)
    {
        return await _mediator.Send(
            new CreateUserCommand(
                req.FirstName,
                req.LastName,
                req.Email,
                req.PhoneNumber,
                req.Designation,
                req.TemporaryPassword,
                req.Role,
                req.DepartmentId
            ),
            ct);
    }
}

public class CreateUserRequest
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Designation { get; set; } = string.Empty;

    public string TemporaryPassword { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public Guid DepartmentId { get; set; }
}