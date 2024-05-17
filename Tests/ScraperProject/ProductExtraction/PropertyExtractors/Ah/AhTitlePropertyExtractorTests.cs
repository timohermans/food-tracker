using Scraper.ProductExtraction.PropertyExtractors;
using Scraper.ProductExtraction.PropertyExtractors.Ah;

namespace Tests.ScraperProject.ProductExtraction.PropertyExtractors.Ah;

public class AhTitlePropertyExtractorTests : ExtractorTestBase
{
    [Test]
    public async Task Extracts_title()
    {
        var extractor = new AhTitlePropertyExtractor(Helper.GetLogger<AhTitlePropertyExtractor>());
        var builder = CreateMinimalValidProductBuilder();
        var document = await GetDocumentForAsync(HtmlFrenchBaguette);

        var result = extractor.Extract(document, builder);

        result.Should().Be(ExtractResult.Success);
        builder.Build().Title.Should().Be("AH Franse baguettes");
    }
}