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
                Url = "https://www.ah.nl/broodjes",
                Content = await File.ReadAllTextAsync("../Data/ah_halfvolle_melk.html")
            });
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
            var job = await postDb.ScrapeJobs.Include(sj => sj.Product).FirstOrDefaultAsync();
            job.Should().NotBeNull();
            job!.Product.Should().NotBeNull();
            var product = job!.Product!;
            product.Title.Should().Be("AH Halfvolle melk");
        }
    }
}
