using AngleSharp.Dom;
using Scraper.UseCases.ProductScrape.ProductExtraction;
using Scraper.UseCases.ProductScrape.ProductExtraction.PropertyExtractors;

namespace Scraper.UseCases.ProductScrape.ProductExtraction.PropertyExtractors.Ah;

public class AhIngredientsPropertyExtractor : IAhPropertyExtractor
{
    private readonly ILogger<AhIngredientsPropertyExtractor> _logger;

    public AhIngredientsPropertyExtractor(ILogger<AhIngredientsPropertyExtractor> logger)
    {
        _logger = logger;
    }

    public ExtractResult Extract(IDocument element, ProductBuilder builder)
    {
        var ingredientIdentifier = "Ingrediënten:";
        var detailsSection = AhUtils.
            QueryDetailsSection(element.Body);
        var productInfoBlocks = detailsSection?
            .QuerySelectorAll(".product-info-content-block");
        var ingredientsElement = productInfoBlocks?
            .FirstOrDefault(p => p.TextContent.Contains(ingredientIdentifier))?
            .QuerySelector("p");

        if (ingredientsElement is null && !(detailsSection?.TextContent.Contains("kcal") ?? false))
        {
            _logger.LogError("No ingredients found while there is a kcal found in text. Parsing must be broken!");
            return ExtractResult.Fail;
        }

        if (ingredientsElement is null)
        {
            _logger.LogWarning("No ingredients found");
            return ExtractResult.NotFound;
        }

        var ingredientsText = ProductBuilder.Clean(ingredientsElement.TextContent);
        var endIndex = ingredientsText.IndexOf("."); // sometimes there are useless texts at the end, like "Waarvan toegevoegde suikers..."
        if (endIndex > -1)
        {
            ingredientsText = ingredientsText.Substring(ingredientIdentifier.Length + 1, endIndex - ingredientIdentifier.Length - 1);
        }

        var ingredients = ingredientsText.Split([",", ";"], StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToList();

        builder.Ingredients(ingredients);

        return ExtractResult.Success;
    }
}