using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using BytesRewards.Service.Infrastructure;
using BytesRewards.Service.Users.Domain;
using BytesRewards.Service.Departments.Domain;

namespace BytesRewards.Service.Infrastructure;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Ensure database is created
        await context.Database.MigrateAsync();

        

        // Seed Departments if empty
        if (!await context.Departments.AnyAsync())
        {
            context.Departments.AddRange(
                new Department { Id = Guid.NewGuid(), Name = "IT", Description = "Information Technology" },
                new Department { Id = Guid.NewGuid(), Name = "HR", Description = "Human Resources" },
                new Department { Id = Guid.NewGuid(), Name = "Finance", Description = "Finance Department" },
                new Department { Id = Guid.NewGuid(), Name = "Marketing", Description = "Marketing Department" }
            );
            await context.SaveChangesAsync();
        }
    }
}