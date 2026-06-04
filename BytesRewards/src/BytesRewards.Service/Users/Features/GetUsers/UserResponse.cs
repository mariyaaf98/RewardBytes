namespace BytesRewards.Service.Users.Features.GetUsers;

public sealed class UserResponse
{
    public Guid Id { get; set; }

    public string EmployeeId { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Designation { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public string RoleName { get; set; } = string.Empty;

    public Guid DepartmentId { get; set; }

    public string DepartmentName { get; set; } = string.Empty;
}