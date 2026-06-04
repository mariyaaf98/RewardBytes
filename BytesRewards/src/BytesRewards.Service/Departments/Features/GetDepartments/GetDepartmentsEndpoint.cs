using FastEndpoints;

using AppWeaver.Mediator;

namespace BytesRewards.Service.Departments.Features.GetDepartments;

public sealed class GetDepartmentsEndpoint(
    IMediator mediator
)
    : EndpointWithoutRequest<List<DepartmentResponse>>
{
    public override void Configure()
    {
        Get("/departments");

        AllowAnonymous();
        
        Description(x =>
       x.WithTags("02 - Departments"));
    }

    public override async Task HandleAsync(
        CancellationToken ct
    )
    {
        var result = await mediator.Send(
            new GetDepartmentsQuery(),
            ct);

        Response = result;
    }
}