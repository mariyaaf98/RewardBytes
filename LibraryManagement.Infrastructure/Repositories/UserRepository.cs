using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.UserEntity;
using LibraryManagement.Infrastructure.Data;

namespace LibraryManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<User> GetUsers()
    {
        return _context.User.ToList();
    }

    public User CreateUser(User user)
    {
        _context.User.Add(user);
        _context.SaveChanges();
        return user;
    }
}