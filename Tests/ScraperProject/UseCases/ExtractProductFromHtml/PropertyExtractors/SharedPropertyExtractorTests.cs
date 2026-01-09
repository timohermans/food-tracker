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
    public void NutritionTableConverter_ConvertToKiloCalories_ReturnsExpectedValue(string input, double expected)
    {
        // Act
        var result = NutritionTableConverter.ConvertToKiloCalories(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
}