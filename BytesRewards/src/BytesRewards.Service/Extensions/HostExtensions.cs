using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BytesRewards.Service.Extensions;

public static class HostExtensions
{
    public static async Task MigrateDatabaseAsync<TContext>(this IHost host)
        where TContext :  Microsoft.EntityFrameworkCore.DbContext
    {
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        await context.Database.MigrateAsync();
    }

    public static async Task SeedDatabaseAsync<TContext>(
            this IHost host, Func<TContext, Task> seeder)
            where TContext :  Microsoft.EntityFrameworkCore.DbContext
    {
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        await seeder(context);
    }
}