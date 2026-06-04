public class UpdateUserRequest
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Designation { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public Guid DepartmentId { get; set; }
}