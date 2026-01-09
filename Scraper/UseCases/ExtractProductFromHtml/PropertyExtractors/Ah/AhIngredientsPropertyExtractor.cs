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

    public ExtractResult Extract(IDocument document, ProductBuilder builder)
    {
        var ingredientIdentifier = "Ingrediënten:";
        var detailsSection = AhUtils.
            QueryDetailsSection(document.Body);
        var productInfoBlocks = detailsSection?
            .QuerySelectorAll(".product-info-content-block");
        var ingredientsElement = productInfoBlocks?
            .FirstOrDefault(p => p.TextContent.Contains(ingredientIdentifier))?
            .QuerySelector("p");

        if (ingredientsElement is null)
        {
            _logger.LogWarning("No ingredients found");
            return ExtractResult.NotFound;
        }

        var ingredientsText = ProductBuilder.Clean(ingredientsElement.TextContent);
        ingredientsText = ingredientsText.Replace(ingredientIdentifier, "").Trim();
        var endIndex = ingredientsText.IndexOf("."); // sometimes there are useless texts at the end, like "Waarvan toegevoegde suikers..."
        if (endIndex > -1)
        {
            ingredientsText = ingredientsText.Substring(0, endIndex);
        }

        List<string> ingredients = [];
        StringBuilder ingredient = new();
        bool isInComponents = false;
        char nonBreakingSpace = '\u00A0';
        char[] firstIngredientSeparators = [',', ';', default];
        char[] secondIngredientSeparators = [' ', nonBreakingSpace, '.', default];
        for (int i = 0; i <= ingredientsText.Length; i++)
        {
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
                ingredients.Add(ingredient.ToString().RemoveSpecialCharacters());
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