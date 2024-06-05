using Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors;

namespace Tests.ScraperProject.UseCases.ExtractProductFromHtml;

public class StringExtensionsTests
{
    [Test]
    [TestCase("maïskorrel", "maïskorrel")]
    [TestCase("magere melk", "magere melk")]
    [TestCase("varkenscollageenª", "varkenscollageen")]
    [TestCase("verdikkingsmiddel (guarpitmeel [E412])", "verdikkingsmiddel (guarpitmeel [E412])")]
    public void Removes_special_characters_successfully(string initial, string expected)
    {
        var result = initial.RemoveSpecialCharacters();
        result.Should().Be(expected);
    }
}