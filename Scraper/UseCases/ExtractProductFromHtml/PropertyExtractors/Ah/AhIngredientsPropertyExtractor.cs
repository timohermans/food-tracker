using System.Text;
using AngleSharp.Dom;
using Core.Data.Types;

namespace Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors.Ah;

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

        List<string> ingredients = [];
        StringBuilder ingredient = new();
        bool isInComponents = false;
        for (int i = 0; i <= ingredientsText.Length; i++)
        {
            char[] firstIngredientSeparators = [',', ';', default];
            char[] secondIngredientSeparators = [' ', '.', default];

            char ingrChar = ingredientsText.ElementAtOrDefault(i);
            char nextIngrChar = ingredientsText.ElementAtOrDefault(i + 1);

            if (ingrChar == '(')
            {
                isInComponents = true;
            }

            if (ingrChar == ')')
            {
                isInComponents = false;
            }

            if (!isInComponents &&
                secondIngredientSeparators.Contains(nextIngrChar) &&
                firstIngredientSeparators.Contains(ingrChar))
            {
                ingredients.Add(ingredient.ToString());
                ingredient = new StringBuilder();
                i++;
            }
            else
            {
                ingredient.Append(ingrChar);
            }

        }

        builder.Ingredients(ingredients);

        return ExtractResult.Success;
    }
}