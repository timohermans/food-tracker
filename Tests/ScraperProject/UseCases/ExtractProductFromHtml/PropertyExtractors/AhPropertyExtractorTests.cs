// csharp

using System.Linq;
using Core.Data.Types;
using Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors;
using Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors.Ah;
using Tests.ScraperProject.ExtractProductFromHtml;
using NUnit.Framework;

namespace Tests.ScraperProject.UseCases.ExtractProductFromHtml.PropertyExtractors;

public class AhPropertyExtractorTests : ExtractorTestBase
{
    private const string HtmlFrenchBaguettePath = "ScraperProject/Data/ah_franse_baguettes.html";
    private const string HtmlAardappelballetjes = "ScraperProject/Data/ah_aardappelballetjes.html";
    private const string HtmlMilk = "ScraperProject/Data/ah_halfvolle_melk.html";
    private const string HtmlSchrijfblok = "ScraperProject/Data/ah_schrijfblok_a4.html";
    private const string HtmlYoghurtVanille = "ScraperProject/Data/ah_halfvolle_yoghurt_vanille.html";

    [Test]
    public async Task
        Given_french_baguette_html_When_extracting_title_with_AhTitlePropertyExtractor_Then_title_is_expected()
    {
        var html = await File.ReadAllTextAsync(HtmlFrenchBaguettePath);
        var builder = CreateMinimalValidProductBuilder();
        var document = await GetDocumentForAsync(html);

        var extractor = new AhTitlePropertyExtractor(Helper.GetLogger<AhTitlePropertyExtractor>());
        var result = extractor.Extract(document, builder);

        Assert.That(result, Is.EqualTo(ExtractResult.Success));
        var product = builder.Build();
        Assert.That(product.Title, Is.EqualTo("AH Franse baguettes"));
    }

    [Test]
    public async Task
        Given_french_baguette_html_When_extracting_price_with_AhPricePropertyExtractor_Then_price_is_expected()
    {
        var html = await File.ReadAllTextAsync(HtmlFrenchBaguettePath);
        var builder = CreateMinimalValidProductBuilder();
        var document = await GetDocumentForAsync(html);

        var extractor = new AhPricePropertyExtractor(Helper.GetLogger<AhPricePropertyExtractor>());
        var result = extractor.Extract(document, builder);

        Assert.That(result, Is.EqualTo(ExtractResult.Success));
        var product = builder.Build();
        Assert.That(product.Price, Is.EqualTo(0.75M));
    }

    [Test]
    public async Task
        Given_french_baguette_html_When_extracting_unit_size_with_AhUnitSizePropertyExtractor_Then_unit_size_is_expected()
    {
        var html = await File.ReadAllTextAsync(HtmlFrenchBaguettePath);
        var builder = CreateMinimalValidProductBuilder();
        var document = await GetDocumentForAsync(html);

        var extractor = new AhUnitSizePropertyExtractor(Helper.GetLogger<AhUnitSizePropertyExtractor>());
        var result = extractor.Extract(document, builder);

        Assert.That(result, Is.EqualTo(ExtractResult.Success));
        var product = builder.Build();
        Assert.That(product.UnitSize, Is.EqualTo("2 stuks"));
    }

    [Test]
    public async Task
        Given_french_baguette_html_When_extracting_nutriscore_with_AhNutriscorePropertyExtractor_Then_nutriscore_is_expected()
    {
        var html = await File.ReadAllTextAsync(HtmlFrenchBaguettePath);
        var builder = CreateMinimalValidProductBuilder();
        var document = await GetDocumentForAsync(html);

        var extractor = new AhNutriscorePropertyExtractor(Helper.GetLogger<AhNutriscorePropertyExtractor>());
        var result = extractor.Extract(document, builder);

        Assert.That(result, Is.EqualTo(ExtractResult.Success));
        var product = builder.Build();
        Assert.That(product.Nutriscore, Is.EqualTo(Nutriscore.C));
    }

    [Test]
    public async Task
        Given_french_baguette_html_When_extracting_summary_with_AhSummaryPropertyExtractor_Then_summary_is_expected()
    {
        var html = await File.ReadAllTextAsync(HtmlFrenchBaguettePath);
        var builder = CreateMinimalValidProductBuilder();
        var document = await GetDocumentForAsync(html);

        var extractor = new AhSummaryPropertyExtractor(Helper.GetLogger<AhSummaryPropertyExtractor>());
        var result = extractor.Extract(document, builder);

        Assert.That(result, Is.EqualTo(ExtractResult.Success));
        var product = builder.Build();
        Assert.That(product.Summary,
            Is.EqualTo(
                "<p>Bak thuis zelf verse Franse baguettes af als knapperig onderdeel van elke maaltijd. Lekker bij het ontbijt of de lunch, maar ook bij een feestje of BBQ.</p> <ul> <li>Klaar in 10-12 minuten</li> <li>Altijd vers op tafel, op elk moment van de dag</li> </ul> "));
    }

    [Test]
    public async Task
        Given_french_baguette_html_When_extracting_ingredients_with_AhIngredientsPropertyExtractor_Then_ingredients_are_expected()
    {
        var html = await File.ReadAllTextAsync(HtmlFrenchBaguettePath);
        var builder = CreateMinimalValidProductBuilder();
        var document = await GetDocumentForAsync(html);

        var extractor = new AhIngredientsPropertyExtractor(Helper.GetLogger<AhIngredientsPropertyExtractor>());
        var result = extractor.Extract(document, builder);

        Assert.That(result, Is.EqualTo(ExtractResult.Success));
        var product = builder.Build();
        Assert.That(product.Ingredients, Is.Not.Null);
        var names = product.Ingredients.Select(i => i.Name).ToList();
        Assert.That(names.Count, Is.EqualTo(7));
        Assert.That(names, Is.EquivalentTo(new[]
        {
            "tarwebloem",
            "water",
            "gist",
            "gefermenteerd tarwemeel",
            "tarwemoutmeel",
            "zout",
            "antioxidant (ascorbinezuur [E300])"
        }));
    }

    [Test]
    public async Task
        Given_aardappelballetjes_html_When_extracting_ingredients_with_AhIngredientsPropertyExtractor_Then_ingredients_are_expected()
    {
        var html = await File.ReadAllTextAsync(HtmlAardappelballetjes);
        var builder = CreateMinimalValidProductBuilder();
        var document = await GetDocumentForAsync(html);

        var extractor = new AhIngredientsPropertyExtractor(Helper.GetLogger<AhIngredientsPropertyExtractor>());
        var result = extractor.Extract(document, builder);

        Assert.That(result, Is.EqualTo(ExtractResult.Success));
        var product = builder.Build();
        Assert.That(product.Ingredients, Is.Not.Null);
        var names = product.Ingredients.Select(i => i.Name).ToList();
        Assert.That(names.Count, Is.EqualTo(6));
        Assert.That(names, Is.EquivalentTo(new[]
        {
            "89% aardappel",
            "6,4% aardappelvlok",
            "plantaardige olie (4,1% zonnebloem, raap)",
            "zout",
            "stabilisator (hydroxypropylmethylcellulose [E464])",
            "specerijenextract"
        }));
    }

    [Test]
    public async Task
        Given_french_baguette_html_When_extracting_nutrition_with_AhNutritionExtractor_Then_nutrition_is_expected()
    {
        var html = await File.ReadAllTextAsync(HtmlFrenchBaguettePath);
        var builder = CreateMinimalValidProductBuilder();
        var document = await GetDocumentForAsync(html);

        var extractor = new AhNutritionExtractor(Helper.GetLogger<AhNutritionExtractor>());
        var result = extractor.Extract(document, builder);

        Assert.That(result, Is.EqualTo(ExtractResult.Success));
        var product = builder.Build();
        Assert.That(product.NutritionInfo, Is.Not.Null);
        Assert.That(product.NutritionInfo!.Per, Is.EqualTo(100));
        Assert.That(product.NutritionInfo.PerUnit, Is.EqualTo(Unit.Grams));
        Assert.That(product.NutritionInfo.PortionRecommended, Is.EqualTo(10));
        Assert.That(product.NutritionInfo.Fats, Is.EqualTo(1));
        Assert.That(product.NutritionInfo.FatsSaturated, Is.EqualTo(0.2));
        Assert.That(product.NutritionInfo.FatsUnsaturated, Is.EqualTo(0.8));
        Assert.That(product.NutritionInfo.Calories, Is.EqualTo(241));
        Assert.That(product.NutritionInfo.Carbs, Is.EqualTo(50));
        Assert.That(product.NutritionInfo.Sugars, Is.EqualTo(3.3));
        Assert.That(product.NutritionInfo.Fibres, Is.EqualTo(1.7));
        Assert.That(product.NutritionInfo.Proteines, Is.EqualTo(7.2));
        Assert.That(product.NutritionInfo.Salts, Is.EqualTo(0.9));
        Assert.That(product.NutritionInfo.PreparationState, Is.EqualTo(PreparationState.Unprepared));
    }

    [Test]
    public async Task Given_milk_html_When_extracting_title_with_AhTitlePropertyExtractor_Then_title_is_expected()
    {
        var html = await File.ReadAllTextAsync(HtmlMilk);
        var builder = CreateMinimalValidProductBuilder();
        var document = await GetDocumentForAsync(html);

        var extractor = new AhTitlePropertyExtractor(Helper.GetLogger<AhTitlePropertyExtractor>());
        var result = extractor.Extract(document, builder);

        Assert.That(result, Is.EqualTo(ExtractResult.Success));
        var product = builder.Build();
        Assert.That(product.Title, Is.EqualTo("AH Halfvolle melk"));
    }

    [Test]
    public async Task Given_milk_html_When_extracting_price_with_AhPricePropertyExtractor_Then_price_is_expected()
    {
        var html = await File.ReadAllTextAsync(HtmlMilk);
        var builder = CreateMinimalValidProductBuilder();
        var document = await GetDocumentForAsync(html);

        var extractor = new AhPricePropertyExtractor(Helper.GetLogger<AhPricePropertyExtractor>());
        var result = extractor.Extract(document, builder);

        Assert.That(result, Is.EqualTo(ExtractResult.Success));
        var product = builder.Build();
        Assert.That(product.Price, Is.EqualTo(1.79M));
    }

    [Test]
    public async Task
        Given_milk_html_When_extracting_unit_size_with_AhUnitSizePropertyExtractor_Then_unit_size_is_expected()
    {
        var html = await File.ReadAllTextAsync(HtmlMilk);
        var builder = CreateMinimalValidProductBuilder();
        var document = await GetDocumentForAsync(html);

        var extractor = new AhUnitSizePropertyExtractor(Helper.GetLogger<AhUnitSizePropertyExtractor>());
        var result = extractor.Extract(document, builder);

        Assert.That(result, Is.EqualTo(ExtractResult.Success));
        var product = builder.Build();
        Assert.That(product.UnitSize, Is.EqualTo("1,5 l"));
    }

    [Test]
    public async Task
        Given_milk_html_When_extracting_nutriscore_with_AhNutriscorePropertyExtractor_Then_nutriscore_is_expected()
    {
        var html = await File.ReadAllTextAsync(HtmlMilk);
        var builder = CreateMinimalValidProductBuilder();
        var document = await GetDocumentForAsync(html);

        var extractor = new AhNutriscorePropertyExtractor(Helper.GetLogger<AhNutriscorePropertyExtractor>());
        var result = extractor.Extract(document, builder);

        Assert.That(result, Is.EqualTo(ExtractResult.Success));
        var product = builder.Build();
        Assert.That(product.Nutriscore, Is.EqualTo(Nutriscore.B));
    }

    [Test]
    public async Task Given_milk_html_When_extracting_summary_with_AhSummaryPropertyExtractor_Then_summary_is_expected()
    {
        var html = await File.ReadAllTextAsync(HtmlMilk);
        var builder = CreateMinimalValidProductBuilder();
        var document = await GetDocumentForAsync(html);

        var extractor = new AhSummaryPropertyExtractor(Helper.GetLogger<AhSummaryPropertyExtractor>());
        var result = extractor.Extract(document, builder);

        Assert.That(result, Is.EqualTo(ExtractResult.Success));
        var product = builder.Build();
        Assert.That(product.Summary, Is.EqualTo("<p>Een koud glas halfvolle melk is op elk moment van de dag heerlijk. En ook nog eens heel gezond.</p> <ul> <li>Gezond</li> <li>Vers</li> <li>Lekker op elk moment v/d dag</li> </ul> "));
    }

    [Test]
    public async Task
        Given_milk_html_When_extracting_ingredients_with_AhIngredientsPropertyExtractor_Then_result_is_NotFound_and_ingredients_are_null()
    {
        var html = await File.ReadAllTextAsync(HtmlMilk);
        var builder = CreateMinimalValidProductBuilder();
        var document = await GetDocumentForAsync(html);

        var extractor = new AhIngredientsPropertyExtractor(Helper.GetLogger<AhIngredientsPropertyExtractor>());
        var result = extractor.Extract(document, builder);

        Assert.That(result, Is.EqualTo(ExtractResult.Success));
        var product = builder.Build();
        Assert.That(product.Ingredients, Has.Some.Property(nameof(Ingredient.Name)).EqualTo("halfvolle melk"));
    }

    [Test]
    public async Task Given_milk_html_When_extracting_nutrition_with_AhNutritionExtractor_Then_nutrition_is_expected()
    {
        var html = await File.ReadAllTextAsync(HtmlMilk);
        var builder = CreateMinimalValidProductBuilder();
        var document = await GetDocumentForAsync(html);

        var extractor = new AhNutritionExtractor(Helper.GetLogger<AhNutritionExtractor>());
        var result = extractor.Extract(document, builder);

        Assert.That(result, Is.EqualTo(ExtractResult.Success));
        var product = builder.Build();
        Assert.That(product.NutritionInfo, Is.Not.Null);
        Assert.That(product.NutritionInfo!.Per, Is.EqualTo(100));
        Assert.That(product.NutritionInfo.PerUnit, Is.EqualTo(Unit.Milliliters));
        Assert.That(product.NutritionInfo.PortionRecommended, Is.EqualTo(200));
        Assert.That(product.NutritionInfo.Fats, Is.EqualTo(1.5));
        Assert.That(product.NutritionInfo.FatsSaturated, Is.EqualTo(1.1));
        Assert.That(product.NutritionInfo.FatsUnsaturated, Is.EqualTo(0.4));
        Assert.That(product.NutritionInfo.Calories, Is.EqualTo(48));
        Assert.That(product.NutritionInfo.Carbs, Is.EqualTo(5));
        Assert.That(product.NutritionInfo.Sugars, Is.EqualTo(5));
        Assert.That(product.NutritionInfo.Fibres, Is.EqualTo(0));
        Assert.That(product.NutritionInfo.Proteines, Is.EqualTo(3.5));
        Assert.That(product.NutritionInfo.Salts, Is.EqualTo(0.13));
        Assert.That(product.NutritionInfo.PreparationState, Is.EqualTo(PreparationState.Unprepared));
    }

    [Test]
    public async Task
        Given_schrijfblok_html_When_extracting_ingredients_with_AhIngredientsPropertyExtractor_Then_result_is_NotFound_and_ingredients_are_null()
    {
        var html = await File.ReadAllTextAsync(HtmlSchrijfblok);
        var builder = CreateMinimalValidProductBuilder();
        var document = await GetDocumentForAsync(html);

        var extractor = new AhIngredientsPropertyExtractor(Helper.GetLogger<AhIngredientsPropertyExtractor>());
        var result = extractor.Extract(document, builder);

        Assert.That(result, Is.EqualTo(ExtractResult.NotFound));
        var product = builder.Build();
        Assert.That(product.Ingredients, Is.Null);
    }

    [Test]
    public async Task
        Given_yoghurt_vanille_html_When_extracting_ingredients_with_AhIngredientsPropertyExtractor_Then_ingredients_contain_expected_items()
    {
        var html = await File.ReadAllTextAsync(HtmlYoghurtVanille);
        var builder = CreateMinimalValidProductBuilder();
        var document = await GetDocumentForAsync(html);

        var extractor = new AhIngredientsPropertyExtractor(Helper.GetLogger<AhIngredientsPropertyExtractor>());
        var result = extractor.Extract(document, builder);

        Assert.That(result, Is.EqualTo(ExtractResult.Success));
        var product = builder.Build();
        Assert.That(product.Ingredients, Is.Not.Null);
        var names = product.Ingredients.Select(i => i.Name).ToList();
        Assert.That(names, Does.Contain("suiker"));
        Assert.That(names, Does.Contain("halfvolle YOGHURT"));
        Assert.That(names, Does.Contain("gemodificeerd maïszetmeel"));
        Assert.That(names, Does.Contain("aroma"));
        Assert.That(names, Does.Contain("kleurstoffen: curcumine en annato"));
    }
}