using AppWeaver.Mediator.Interfaces;

namespace BytesRewards.Service.Departments.Features.GetDepartments;

public sealed record GetDepartmentsQuery()
    : IQuery<List<DepartmentResponse>>;