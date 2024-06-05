using Core.Data.Types;
using Microsoft.EntityFrameworkCore;
using Scraper.UseCases.ExtractProductFromHtml;

namespace Tests;

public class ProductPersisterTests : IntegrationTestBase
{
    [Test]
    public async Task Updates_product_correctly()
    {
        await using (var preDb = GetDb())
        {
            await preDb.Products.AddAsync(new Product
            {
                Title = "AH Aardappelballetjes",
                Nutriscore = Nutriscore.C,
                Ingredients = [
                    new Ingredient { Name = "suiker" },
                    new Ingredient { Name = "zout" }
                ]
            });
            await preDb.SaveChangesAsync();
        }

        await using (var db = GetDb())
        {
            var persister = new ProductPersister(db);

            await persister.Persist(new Product
            {
                Title = "AH Aardappelballetjes",
                Nutriscore = Nutriscore.B,
                Ingredients = [
                    new Ingredient { Name = "zout"},
                    new Ingredient { Name = "soja"}
                ]
            });
            await db.SaveChangesAsync();
        }

        await using var postDb = GetDb();
        var products = await postDb.Products
                            .Include(p => p.Ingredients)
                            .ToListAsync();
        products.Should().HaveCount(1, "Product should be updated by title, not created additionally");
        var product = products[0];
        product.Title.Should().Be("AH Aardappelballetjes");
        product.Nutriscore.Should().Be(Nutriscore.B);
        (await postDb.Ingredients.CountAsync()).Should().Be(3, "Because I haven't added any cleanup code");
        product.Ingredients.Should().HaveCount(2);
        product.Ingredients.Should().Satisfy(
            i => i.Name == "zout",
            i => i.Name == "soja");
    }
}
