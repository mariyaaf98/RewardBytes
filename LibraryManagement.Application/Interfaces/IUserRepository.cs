using LibraryManagement.Domain.UserEntity;

namespace LibraryManagement.Application.Interfaces;

public interface IUserRepository
{
    List<User> GetUsers();
    User CreateUser(User user);
}