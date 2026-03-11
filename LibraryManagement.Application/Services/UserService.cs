using LibraryManagement.Domain.UserEntity;
using LibraryManagement.Application.Interfaces;

namespace LibraryManagement.Application.Services;

public class UserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public List<User> GetUsers()
    {
        return _repository.GetUsers();
    }

    public User CreateUser(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        return _repository.CreateUser(user);
    }
}