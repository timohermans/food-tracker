using Core.Data.Types;
using Scraper.UseCases.ProductScrape.ProductExtraction.PropertyExtractors;
using Scraper.UseCases.ProductScrape.ProductExtraction.PropertyExtractors.Ah;
using System.Collections;

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
                    (Product p) => { p.Title.Should().Be("AH Franse baguettes"); });
                yield return new TestCaseData(
                    new AhPricePropertyExtractor(Helper.GetLogger<AhPricePropertyExtractor>()),
                    (Product p) => { p.Price.Should().Be(0.75M); });
                yield return new TestCaseData(
                    new AhUnitSizePropertyExtractor(Helper.GetLogger<AhUnitSizePropertyExtractor>()),
                    (Product p) => { p.UnitSize.Should().Be("2 stuks"); });
                yield return new TestCaseData(
                    new AhNutriscorePropertyExtractor(Helper.GetLogger<AhNutriscorePropertyExtractor>()),
                    (Product p) => { p.Nutriscore.Should().Be(Nutriscore.C); });
                yield return new TestCaseData(
                    new AhSummaryPropertyExtractor(Helper.GetLogger<AhSummaryPropertyExtractor>()),
                    (Product p) => { p.Summary.Should().Be("<p>Bak thuis zelf verse Franse baguettes af als knapperig onderdeel van elke maaltijd. Lekker bij het ontbijt of de lunch, maar ook bij een feestje of BBQ.</p> <ul> <li>Klaar in 10-12 minuten</li> <li>Altijd vers op tafel, op elk moment van de dag</li> </ul> "); });
                yield return new TestCaseData(
                    new AhIngredientsPropertyExtractor(Helper.GetLogger<AhIngredientsPropertyExtractor>()),
                    (Product p) =>
                    {
                        p.Ingredients.Should()
                                        .HaveCount(7)
                                        .And
                                        .Satisfy(
                                            i => i.Name == "tarwebloem",
                                            i => i.Name == "water",
                                            i => i.Name == "gist",
                                            i => i.Name == "gefermenteerd tarwemeel",
                                            i => i.Name == "tarwemoutmeel",
                                            i => i.Name == "zout",
                                            i => i.Name == "antioxidant (ascorbinezuur [E300])");
                    });
                yield return new TestCaseData(
                    new AhNutritionExtractor(Helper.GetLogger<AhNutritionExtractor>()),
                    (Product p) =>
                    {
                        p.NutritionInfo.Should().NotBeNull();
                        p.NutritionInfo!.Per.Should().Be(100);
                        p.NutritionInfo.PerUnit.Should().Be(Unit.Grams);
                        p.NutritionInfo.PortionRecommended.Should().Be(10);
                        p.NutritionInfo.Fats.Should().Be(1);
                        p.NutritionInfo.FatsSaturated.Should().Be(0.2);
                        p.NutritionInfo.FatsUnsaturated.Should().Be(0.8);
                        p.NutritionInfo.Calories.Should().Be(241);
                        p.NutritionInfo.Carbs.Should().Be(50);
                        p.NutritionInfo.Sugars.Should().Be(3.3);
                        p.NutritionInfo.Fibres.Should().Be(1.7);
                        p.NutritionInfo.Proteines.Should().Be(7.2);
                        p.NutritionInfo.Salts.Should().Be(0.9);
                    });
            }
        }
    }

    [Test]
    [TestCaseSource(typeof(TestData), nameof(TestData.TestCases))]
    public async Task Extracts_property_successfully(IProductPropertyExtractor extractor, Action<Product> assertFn)
    {
        var builder = CreateMinimalValidProductBuilder();
        var document = await GetDocumentForAsync(HtmlFrenchBaguette);

        var result = extractor?.Extract(document, builder);

        result.Should().Be(ExtractResult.Success);
        var product = builder.Build();
        assertFn(product);
    }
}