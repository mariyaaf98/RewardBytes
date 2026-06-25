using Microsoft.EntityFrameworkCore;
using AppWeaver.Repository.EfCore;
using AppWeaver.Contexts;
using AppWeaver.DomainAbstraction.Entities;
using BytesRewards.Service.Users.Domain;
using BytesRewards.Service.Departments.Domain;
using BytesRewards.Service.Appreciations.Domain;
using BytesRewards.Service.RewardCategories.Domain;
using BytesRewards.Service.Rewards.Domain;
using BytesRewards.Service.Wallets.Domain;

using BytesRewards.Service.RewardsCatalog.Domain;
using BytesRewards.Service.Redemptions.Domain;
using BytesRewards.Service.Designations.Domain;
using BytesRewards.Service.Notifications.Domain;

namespace BytesRewards.Service.Infrastructure;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    ICurrentTenant? tenant = null
) : ContextAwareDbContext<ApplicationDbContext>(options, tenant)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Appreciation> Appreciations => Set<Appreciation>();

    public DbSet<RewardCategory> RewardCategories => Set<RewardCategory>();

    public DbSet<Reward> Rewards => Set<Reward>();

    public DbSet<Wallet> Wallets => Set<Wallet>();

    public DbSet<RewardItem> RewardItems=> Set<RewardItem>();

    public DbSet<Redemption> Redemptions => Set<Redemption>();

    public DbSet<Designation> Designations => Set<Designation>();

    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
                                typeof(ApplicationDbContext).Assembly);

        // Provider-aware DateTimeOffset mapping
        if (Database.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL")
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties()
                        .Where(p => p.ClrType == typeof(DateTimeOffset)
                                 || p.ClrType == typeof(DateTimeOffset?)))
                {
                    property.SetColumnType("timestamptz");
                }
            }
        }

        modelBuilder.ApplyCrossCuttingBehaviors(this);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IConcurrencyTracked).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<byte[]>("RowVersion")
                    .IsConcurrencyToken()
                    .ValueGeneratedNever();
            }
        }

        base.OnModelCreating(modelBuilder);
    }
}