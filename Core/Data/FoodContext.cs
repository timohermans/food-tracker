using Core.Data.Types;
using Microsoft.EntityFrameworkCore;

namespace Core.Data;

public class FoodContext : DbContext
{
    public DbSet<ProductScrapeJob> ScrapeJobs { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<NutritionInfo> NutritionInfos { get; set; }
    public DbSet<FoodEntry> FoodEntries { get; set; }

    public FoodContext(DbContextOptions<FoodContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FoodContext).Assembly);
    }

    public override int SaveChanges()
    {
        BeforeSaveChanges();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        BeforeSaveChanges();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void BeforeSaveChanges()
    {
        ChangeTracker.Entries<IAuditable>()
            .ToList()
            .ForEach(entry =>
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.Now;
                }
            });
    }
}