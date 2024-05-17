using Scraper.ProductExtration;
using Scraper.Services;

namespace Tests.ScraperProject;

public class ProductExtractorTests
{
    [Test]
    public async Task Extracts_title()
    {
        var content = await File.ReadAllTextAsync("ScraperProject/Data/ah_franse_baguettes.html");
        var extractor = new ProductExtractor(Helper.GetLogger<ProductExtractor>());

        var result = await extractor.ExtractAsync(content);

        result.Should().BeOfType<ProductSuccess>();
        result.Result!.Title.Should().Be("AH Franse baguettes");
    }

}
