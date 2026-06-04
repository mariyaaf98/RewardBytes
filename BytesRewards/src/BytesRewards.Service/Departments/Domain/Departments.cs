using BytesRewards.Service.Common;

namespace BytesRewards.Service.Departments.Domain;
public class Department : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}