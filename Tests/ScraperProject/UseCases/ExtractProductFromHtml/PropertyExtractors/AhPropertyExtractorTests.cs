using Core.Data.Types;
using Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors;
using Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors.Ah;
using System.Collections;

namespace Tests.ScraperProject.ExtractProductFromHtml.PropertyExtractors;

public class AhPropertyExtractorTests : ExtractorTestBase
{
    private static class TestData
    {
        public static IEnumerable TestCases
        {
            get
            {
                var htmlFrenchBaguette = "ScraperProject/Data/ah_franse_baguettes.html";
                yield return new TestCaseData(htmlFrenchBaguette, ExtractResult.Success,
                    new AhTitlePropertyExtractor(Helper.GetLogger<AhTitlePropertyExtractor>()),
                    (Product p) => { p.Title.Should().Be("AH Franse baguettes"); });
                yield return new TestCaseData(htmlFrenchBaguette, ExtractResult.Success,
                    new AhPricePropertyExtractor(Helper.GetLogger<AhPricePropertyExtractor>()),
                    (Product p) => { p.Price.Should().Be(0.75M); });
                yield return new TestCaseData(htmlFrenchBaguette, ExtractResult.Success,
                    new AhUnitSizePropertyExtractor(Helper.GetLogger<AhUnitSizePropertyExtractor>()),
                    (Product p) => { p.UnitSize.Should().Be("2 stuks"); });
                yield return new TestCaseData(htmlFrenchBaguette, ExtractResult.Success,
                    new AhNutriscorePropertyExtractor(Helper.GetLogger<AhNutriscorePropertyExtractor>()),
                    (Product p) => { p.Nutriscore.Should().Be(Nutriscore.C); });
                yield return new TestCaseData(htmlFrenchBaguette, ExtractResult.Success,
                    new AhSummaryPropertyExtractor(Helper.GetLogger<AhSummaryPropertyExtractor>()),
                    (Product p) => { p.Summary.Should().Be("<p>Bak thuis zelf verse Franse baguettes af als knapperig onderdeel van elke maaltijd. Lekker bij het ontbijt of de lunch, maar ook bij een feestje of BBQ.</p> <ul> <li>Klaar in 10-12 minuten</li> <li>Altijd vers op tafel, op elk moment van de dag</li> </ul> "); });
                yield return new TestCaseData(htmlFrenchBaguette, ExtractResult.Success,
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
                    
                yield return new TestCaseData("ScraperProject/Data/ah_aardappelballetjes.html", ExtractResult.Success,
                    new AhIngredientsPropertyExtractor(Helper.GetLogger<AhIngredientsPropertyExtractor>()),
                    (Product p) =>
                    {
                        p.Ingredients.Should()
                                        .HaveCount(6)
                                        .And
                                        .Satisfy(
                                            i => i.Name == "88% aardappel",
                                            i => i.Name == "6,4% aardappelvlok",
                                            i => i.Name == "plantaardige olie (4,7% zonnebloem, raap)",
                                            i => i.Name == "zout",
                                            i => i.Name == "stabilisator (hydroxypropylmethylcellulose [E464])",
                                            i => i.Name == "specerijenextract");
                    });
                yield return new TestCaseData(htmlFrenchBaguette, ExtractResult.Success,
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
                        p.NutritionInfo.PreparationState.Should().Be(PreparationState.Unprepared);
                    });


                var htmlMilk = "ScraperProject/Data/ah_halfvolle_melk.html";
                yield return new TestCaseData(htmlMilk, ExtractResult.Success,
                    new AhTitlePropertyExtractor(Helper.GetLogger<AhTitlePropertyExtractor>()),
                    (Product p) => { p.Title.Should().Be("AH Houdbare halfvolle melk"); });
                yield return new TestCaseData(htmlMilk, ExtractResult.Success,
                    new AhPricePropertyExtractor(Helper.GetLogger<AhPricePropertyExtractor>()),
                    (Product p) => { p.Price.Should().Be(1.05M); });
                yield return new TestCaseData(htmlMilk, ExtractResult.Success,
                    new AhUnitSizePropertyExtractor(Helper.GetLogger<AhUnitSizePropertyExtractor>()),
                    (Product p) => { p.UnitSize.Should().Be("1 l"); });
                yield return new TestCaseData(htmlMilk, ExtractResult.Success,
                    new AhNutriscorePropertyExtractor(Helper.GetLogger<AhNutriscorePropertyExtractor>()),
                    (Product p) => { p.Nutriscore.Should().Be(Nutriscore.B); });
                yield return new TestCaseData(htmlMilk, ExtractResult.Success,
                    new AhSummaryPropertyExtractor(Helper.GetLogger<AhSummaryPropertyExtractor>()),
                    (Product p) => { p.Summary.Should().Be("<p>Houdbare halfvolle melk van 100% weidemelk, want de lekkerste melk komt van de gelukkigste koeien.</p><p></p><ul><li>Bevat calcium</li><li>Hersluitbaar</li><li>Prijsfavorieten: topkwaliteit en altijd laaggeprijsd</li></ul><p></p>"); });
                yield return new TestCaseData(htmlMilk, ExtractResult.NotFound,
                    new AhIngredientsPropertyExtractor(Helper.GetLogger<AhIngredientsPropertyExtractor>()),
                    (Product p) =>
                    {
                        p.Ingredients.Should().BeNull();
                    });
                yield return new TestCaseData(htmlMilk, ExtractResult.Success,
                    new AhNutritionExtractor(Helper.GetLogger<AhNutritionExtractor>()),
                    (Product p) =>
                    {
                        p.NutritionInfo.Should().NotBeNull();
                        p.NutritionInfo!.Per.Should().Be(100);
                        p.NutritionInfo.PerUnit.Should().Be(Unit.Milliliters);
                        p.NutritionInfo.PortionRecommended.Should().Be(200);
                        p.NutritionInfo.Fats.Should().Be(1.5);
                        p.NutritionInfo.FatsSaturated.Should().Be(1.1);
                        p.NutritionInfo.FatsUnsaturated.Should().Be(0.4);
                        p.NutritionInfo.Calories.Should().Be(48);
                        p.NutritionInfo.Carbs.Should().Be(5);
                        p.NutritionInfo.Sugars.Should().Be(5);
                        p.NutritionInfo.Fibres.Should().Be(0);
                        p.NutritionInfo.Proteines.Should().Be(3.5);
                        p.NutritionInfo.Salts.Should().Be(0.13);
                        p.NutritionInfo.PreparationState.Should().Be(PreparationState.Unprepared);
                    });
            }
        }
    }

    [Test]
    [TestCaseSource(typeof(TestData), nameof(TestData.TestCases))]
    public async Task Extracts_property_successfully(string htmlPath, ExtractResult expectedResult, IProductPropertyExtractor extractor, Action<Product> assertFn)
    {
        var html = await File.ReadAllTextAsync(htmlPath);
        var builder = CreateMinimalValidProductBuilder();
        var document = await GetDocumentForAsync(html);

        var result = extractor?.Extract(document, builder);

        result.Should().Be(expectedResult);
        var product = builder.Build();
        assertFn(product);
    }
}