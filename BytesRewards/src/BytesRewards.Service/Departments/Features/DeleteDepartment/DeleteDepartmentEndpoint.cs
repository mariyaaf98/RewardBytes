using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

namespace BytesRewards.Service.Departments.Features.DeleteDepartment;

public sealed class DeleteDepartmentEndpoint(
    IMediator mediator
) : SecureFastEndpoint<DeleteDepartmentRequest, bool>
{
    public override void Configure()
    {
        Delete("/departments/{id}");

        Roles("admin");

        Summary(s => s.Summary = "Delete a department");

        Options(o => o.WithTags("02 - Departments"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy() => new()
    {
        SecurityLevel = SecurityLevel.Internal,
        CachePolicy = CachePolicy.NoStore
    };

    protected override async Task<Result<bool>> ExecuteAsync(
        DeleteDepartmentRequest req,
        CancellationToken ct)
    {
        return await mediator.Send(
            new DeleteDepartmentCommand(req.Id),
            ct);
    }
}
