using System.Collections;
using Core.Data.Types;
using Scraper.ProductExtraction.PropertyExtractors;
using Scraper.ProductExtraction.PropertyExtractors.Ah;

namespace Tests.ScraperProject.ProductExtraction.PropertyExtractors;

public class AhPropertyExtractorTests : ExtractorTestBase
{
    private class TestData
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(
                    new AhTitlePropertyExtractor(Helper.GetLogger<AhTitlePropertyExtractor>()),
                    nameof(Product.Title), "AH Franse baguettes");
                yield return new TestCaseData(
                    new AhPricePropertyExtractor(Helper.GetLogger<AhPricePropertyExtractor>()),
                    nameof(Product.Price), 0.75M);
                yield return new TestCaseData(
                    new AhUnitSizePropertyExtractor(Helper.GetLogger<AhUnitSizePropertyExtractor>()),
                    nameof(Product.UnitSize), "2 stuks");
                yield return new TestCaseData(
                    new AhNutriscorePropertyExtractor(Helper.GetLogger<AhNutriscorePropertyExtractor>()),
                    nameof(Product.Nutriscore), Nutriscore.C);
                yield return new TestCaseData(
                    new AhSummaryPropertyExtractor(Helper.GetLogger<AhSummaryPropertyExtractor>()),
                    nameof(Product.Summary), "<p>Bak thuis zelf verse Franse baguettes af als knapperig onderdeel van elke maaltijd. Lekker bij het ontbijt of de lunch, maar ook bij een feestje of BBQ.</p> <ul> <li>Klaar in 10-12 minuten</li> <li>Altijd vers op tafel, op elk moment van de dag</li> </ul> ");
            }
        }
    }

    [Test]
    [TestCaseSource(typeof(TestData), nameof(TestData.TestCases))]
    public async Task Extracts_property_successfully(IProductPropertyExtractor extractor, string property, object value)
    {
        var builder = CreateMinimalValidProductBuilder();
        var document = await GetDocumentForAsync(HtmlFrenchBaguette);

        var result = extractor?.Extract(document, builder);

        result.Should().Be(ExtractResult.Success);
        var product = builder.Build();
        Helper.GetPropertyValue(product, property).Should().Be(value);
    }
}