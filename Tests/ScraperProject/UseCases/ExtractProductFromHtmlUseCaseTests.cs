using Core.Data.Types;
using Microsoft.EntityFrameworkCore;
using Scraper.UseCases;
using Scraper.UseCases.ExtractProductFromHtml;
using Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors.Ah;

namespace Tests.ScraperProject.UseCases;

public class ExtractProductFromHtmlUseCaseTests : IntegrationTestBase
{
    [Test]
    public async Task Extracts_product_successfully()
    {
        await using (var preDb = GetDb())
        {
            await preDb.ScrapeJobs.AddAsync(new ProductScrapeJob
            {
                Url = "https://www.ah.nl/producten/product/wi107/ah-aardappelballetjes",
                Content = await File.ReadAllTextAsync("ScraperProject/Data/ah_aardappelballetjes.html")
            });
            await preDb.SaveChangesAsync();
        }

        await using (var db = GetDb())
        {
            var productExtractor = new ProductExtractor(TestLogger.Create<ProductExtractor>());
            var logger = TestLogger.Create<ExtractProductFromHtmlUseCase>();
            List<IAhPropertyExtractor> propertyExtractors = [
                new AhIngredientsPropertyExtractor(TestLogger.Create<AhIngredientsPropertyExtractor>()),
                new AhNutriscorePropertyExtractor(TestLogger.Create<AhNutriscorePropertyExtractor>()),
                new AhNutritionExtractor(TestLogger.Create<AhNutritionExtractor>()),
                new AhPricePropertyExtractor(TestLogger.Create<AhPricePropertyExtractor>()),
                new AhSummaryPropertyExtractor(TestLogger.Create<AhSummaryPropertyExtractor>()),
                new AhTitlePropertyExtractor(TestLogger.Create<AhTitlePropertyExtractor>()),
                new AhUnitSizePropertyExtractor(TestLogger.Create<AhUnitSizePropertyExtractor>()),
            ];
            var useCase = new ExtractProductFromHtmlUseCase(db, productExtractor, propertyExtractors, logger);

            await useCase.Invoke();
        }

        await using (var postDb = GetDb())
        {
            var job = await postDb.ScrapeJobs
                        .Include(sj => sj.Product)
                        .ThenInclude(p => p.Ingredients)
                        .Include(sj => sj.Product)
                        .ThenInclude(p => p.NutritionInfo)
                        .FirstOrDefaultAsync();
            job.Should().NotBeNull();
            job!.Product.Should().NotBeNull();
            var product = job!.Product!;
            product.Title.Should().Be("AH Aardappelballetjes");
            product.Ingredients.Should().Satisfy(
                i => i.Name == "89% aardappel",
                i => i.Name == "6,4% aardappelvlok",
                i => i.Name == "plantaardige olie (4,1% zonnebloem, raap)",
                i => i.Name == "zout",
                i => i.Name == "stabilisator (hydroxypropylmethylcellulose [E464])",
                i => i.Name == "specerijenextract"
            );
            product.Nutriscore.Should().Be(Nutriscore.C);
            product.NutritionInfo.Should().NotBeNull();
            product.NutritionInfo!.Per.Should().Be(100);
            product.NutritionInfo.PerUnit.Should().Be(Unit.Grams);
            product.NutritionInfo.Calories.Should().Be(137);
            product.NutritionInfo.Fats.Should().Be(4.4);
            product.NutritionInfo.FatsSaturated.Should().Be(0.5);
            product.NutritionInfo.FatsUnsaturated.Should().Be(3.9);
            product.NutritionInfo.Carbs.Should().Be(21);
            product.NutritionInfo.Sugars.Should().Be(0.7);
            product.NutritionInfo.Fibres.Should().Be(2.2);
            product.NutritionInfo.Proteines.Should().Be(2.2);
            product.NutritionInfo.Salts.Should().Be(0.7);
        }
    }
}
