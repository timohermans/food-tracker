using AngleSharp;
using AngleSharp.Dom;
using Scraper.ProductExtraction;

namespace Tests.ScraperProject.ProductExtraction;

public class ExtractorTestBase
{
    protected string HtmlFrenchBaguette { get; private set; }

    [OneTimeSetUp]
    public async Task Setup()
    {
        HtmlFrenchBaguette = await File.ReadAllTextAsync("ScraperProject/Data/ah_franse_baguettes.html");
    }

    protected async Task<IDocument> GetDocumentForAsync(string html)
    {
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        return await context.OpenAsync(req => req.Content(html));
    }

    protected ProductBuilder CreateMinimalValidProductBuilder()
    {
        var builder = new ProductBuilder();
        builder.Title("Default title");
        return builder;
    }
}