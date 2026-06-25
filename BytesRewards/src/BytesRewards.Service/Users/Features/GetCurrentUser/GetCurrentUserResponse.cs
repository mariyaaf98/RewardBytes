namespace BytesRewards.Service.Users.Features.GetCurrentUser;

public sealed class GetCurrentUserResponse
{
    public Guid Id { get; set; }

    public string EmployeeId { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public Guid DesignationId { get; set; }

    public string DesignationName { get; set; } = string.Empty;

    public string ProfileImageUrl { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public string DepartmentId { get; set; } = string.Empty;

    public string DepartmentName { get; set; } = string.Empty;

    public string RoleName { get; set; } = string.Empty;
}
