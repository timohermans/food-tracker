using Core.Data.Types;
using Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors.Shared;

namespace Tests.ScraperProject.UseCases.ExtractProductFromHtml.PropertyExtractors;

public class SharedPropertyExtractorTests
{
    [Test]
    [TestCase("500 kj (120 kcal)", 120)]
    [TestCase("1023.0 kj (241.0 kcal)", 241)]
    [TestCase("1023.0 kj (241.1 kcal)", 241.1)]
    [TestCase("241.1 kcal", 241.1)]
    [TestCase("241", 241)]
    public void NutritionTableConverter_ParseKiloCaloriesFrom_ReturnsExpectedValue(string input, double expected)
    {
        // Act
        var result = NutritionTableParser.ParseKiloCaloriesFrom(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
    
    [TestCase("75 gram", 75, Unit.Grams)]
    [TestCase("250ml", 250, Unit.Milliliters)]
    [TestCase("Per 15 g** ** 1 portie = 15 g per boterham", 15, Unit.Grams)]
    [TestCase("10 milliliter", 10, Unit.Milliliters)]
    [TestCase("2.5 ml", 2.5, Unit.Milliliters)]
    [TestCase("2,5 ml", 2.5, Unit.Milliliters)]
    public void PortionUnitParser_Parse_WhenInputContainsAmountAndUnit_ThenReturnsParsedValues(
        string input,
        double expectedAmount,
        Unit expectedUnit)
    {
        // Act
        var result = PortionAndUnitParser.Parse(input);

        // Assert
        Assert.That(result.Amount, Is.EqualTo(expectedAmount));
        Assert.That(result.Unit, Is.EqualTo(expectedUnit));
    } 
}