using LibraryManagement.Domain.CommonEntity;

namespace LibraryManagement.Domain.UserEntity;

public class User : BaseEntity
{
    public string Name { get; set; } = "";

    public string Email { get; set; } = "";

    public string Password { get; set; } = "";

    public string Role { get; set; } = "";

    public string Status { get; set; } = "";

    public decimal Fine { get; set; }
}