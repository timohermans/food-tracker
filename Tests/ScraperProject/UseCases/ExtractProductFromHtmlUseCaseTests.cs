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
                        .FirstOrDefaultAsync();
            job.Should().NotBeNull();
            job!.Product.Should().NotBeNull();
            var product = job!.Product!;
            product.Title.Should().Be("AH Aardappelballetjes");
            product.Ingredients.Should().Satisfy(
                i => i.Name == "88% aardappel",
                i => i.Name == "6,4% aardappelvlok",
                i => i.Name == "plantaardige olie (4,7% zonnebloem, raap)",
                i => i.Name == "zout",
                i => i.Name == "stabilisator (hydroxypropylmethylcellulose [E464])",
                i => i.Name == "specerijenextract"
            );
        }
    }
}
