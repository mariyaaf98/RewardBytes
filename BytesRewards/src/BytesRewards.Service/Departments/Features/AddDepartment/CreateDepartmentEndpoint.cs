using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

namespace BytesRewards.Service.Departments.Features.CreateDepartment;

public sealed class CreateDepartmentEndpoint(
    IMediator mediator
)
    : SecureFastEndpoint<CreateDepartmentRequest, Guid>
{
    public override void Configure()
    {
        Post("/departments");

        Roles("admin");

        Options(option =>
            option.WithTags("02 - Departments"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy()
        => new()
        {
            SecurityLevel = SecurityLevel.Internal,
            CachePolicy = CachePolicy.NoStore
        };

    protected override async Task<Result<Guid>> ExecuteAsync(
        CreateDepartmentRequest req,
        CancellationToken ct)
    {
        return await mediator.Send(
            new CreateDepartmentCommand(
                req.Name,
                req.Description
            ),
            ct);
    }
}