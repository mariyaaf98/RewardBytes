namespace BytesRewards.Service.Users.Features.UpdateUser;

public class UpdateUserRequest
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public Guid DesignationId { get; set; }

    public string Role { get; set; } = string.Empty;

    public Guid DepartmentId { get; set; }
}