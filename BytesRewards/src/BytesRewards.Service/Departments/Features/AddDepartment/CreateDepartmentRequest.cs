namespace BytesRewards.Service.Departments.Features.CreateDepartment;

public sealed class CreateDepartmentRequest
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}