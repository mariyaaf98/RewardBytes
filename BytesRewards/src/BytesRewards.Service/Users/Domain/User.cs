using AppWeaver.DomainAbstraction.Aggregates;

using BytesRewards.Service.Departments.Domain;
using BytesRewards.Service.Designations.Domain;

using BytesRewards.Service.Common;

namespace BytesRewards.Service.Users.Domain;

public class User : BaseEntity, IAggregateRoot
{
    public string EmployeeId { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string ProfileImageUrl { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public string KeycloakUserId { get; set; } = string.Empty;

    public Guid DepartmentId { get; set; }

    public Department Department { get; set; } = null!;

    public Guid DesignationId { get; set; }

    public Designation Designation { get; set; } = null!;
}