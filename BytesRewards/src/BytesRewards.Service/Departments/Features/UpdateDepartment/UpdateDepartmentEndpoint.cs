using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

namespace BytesRewards.Service.Departments.Features.UpdateDepartment;

public sealed class UpdateDepartmentEndpoint(
    IMediator mediator
) : SecureFastEndpoint<UpdateDepartmentRequest, bool>
{
    public override void Configure()
    {
        Put("/departments/{id}");

        Roles("admin");

        Summary(s => s.Summary = "Update a department");

        Options(o => o.WithTags("02 - Departments"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy() => new()
    {
        SecurityLevel = SecurityLevel.Internal,
        CachePolicy = CachePolicy.NoStore
    };

    protected override async Task<Result<bool>> ExecuteAsync(
        UpdateDepartmentRequest req,
        CancellationToken ct)
    {
        return await mediator.Send(
            new UpdateDepartmentCommand(
                req.Id,
                req.Name,
                req.Description
            ),
            ct);
    }
}
