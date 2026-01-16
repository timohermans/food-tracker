using Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Tests;

[NonParallelizable]
public class IntegrationTestBase
{
    private DbContextOptions<FoodContext> _options = null!;

    protected FoodContext GetDb() => new(_options);

    [OneTimeSetUp]
    public async Task BeforeAllAsync()
    {
        _options = new DbContextOptionsBuilder<FoodContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=food-test;Username=dev;Password=dev")
            .Options;

        await using var db = GetDb();
        await db.Database.MigrateAsync();
    }

    [SetUp]
    public async Task BeforeEachAsync()
    {
        await using var db = GetDb();

        await db.Database.ExecuteSqlRawAsync("""
            delete from "ScrapeJobs";
            delete from "ProductIngredients";
            delete from "Ingredients";
            delete from "Products";
            delete from "NutritionInfos";
            """);

    }

}
