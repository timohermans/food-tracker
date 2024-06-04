using Scraper.UseCases.ExtractProductFromHtml;
using Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors;
using Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors.Ah;

namespace Tests.ScraperProject;

public class ProductExtractorTests
{
    [Test]
    public async Task Extracts_title()
    {
        var content = await File.ReadAllTextAsync("ScraperProject/Data/ah_franse_baguettes.html");

        List<IProductPropertyExtractor> propertyExtractors =
        [
            new AhTitlePropertyExtractor(Helper.GetLogger<AhTitlePropertyExtractor>()),
            new AhPricePropertyExtractor(Helper.GetLogger<AhPricePropertyExtractor>())
        ];

        var extractor = new ProductExtractor(Helper.GetLogger<ProductExtractor>());

        var result = await extractor.ExtractAsync(content, propertyExtractors);

        result.Should().BeOfType<ProductSuccess>();
        var succesResult = result as ProductSuccess;
        succesResult!.Result.Title.Should().Be("AH Franse baguettes");
    }
}