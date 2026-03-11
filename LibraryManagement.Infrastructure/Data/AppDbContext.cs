using Microsoft.EntityFrameworkCore;
using LibraryManagement.Domain.UserEntity;

namespace LibraryManagement.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    {
    }

    public DbSet<User> User { get; set; }

    // public DbSet<Book> Books { get; set; }

    // public DbSet<Author> Authors { get; set; }

    // public DbSet<Category> Categories { get; set; }

    // public DbSet<SubCategory> SubCategories { get; set; }

    // public DbSet<Copy> Copies { get; set; }

    // public DbSet<Loan> Loans { get; set; }

    // public DbSet<Reservation> Reservations { get; set; }
}