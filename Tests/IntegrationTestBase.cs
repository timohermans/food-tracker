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
        // TODO: Verwisselen met mssql db, zodat alles werkt xD
        _options = new DbContextOptionsBuilder<FoodContext>()
            .UseSqlServer("Server=localhost;User Id=sa;Password=P@ssw0rd;Initial Catalog=FoodTrackerTest;TrustServerCertificate=True")
            .Options;

        await using var db = GetDb();
        await db.Database.MigrateAsync();
    }

    [SetUp]
    public async Task BeforeEachAsync()
    {
        await using var db = GetDb();

        await db.Database.ExecuteSqlRawAsync("""
            delete from ScrapeJobs;
            delete from ProductIngredients;
            delete from Ingredients;
            delete from Products;
            delete from NutritionInfos;
            """);

    }

}
