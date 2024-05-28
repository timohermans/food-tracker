using AngleSharp.Dom;
using Scraper.UseCases.ProductScrape.ProductExtraction;
using Scraper.UseCases.ProductScrape.ProductExtraction.PropertyExtractors;
using System.Text.Json;
using Core.Data.Types;
using System.Text.RegularExpressions;

namespace Scraper.UseCases.ProductScrape.ProductExtraction.PropertyExtractors.Ah;

public class AhNutritionExtractor : IAhPropertyExtractor
{
    private readonly ILogger<AhNutritionExtractor> _logger;

    public AhNutritionExtractor(ILogger<AhNutritionExtractor> logger)
    {
        _logger = logger;
    }

    public ExtractResult Extract(IDocument element, ProductBuilder builder)
    {
        var scriptIdentifier = "window.__INITIAL_STATE__ =";
        var scriptElement = element.Scripts.FirstOrDefault(s => s.InnerHtml.Contains(scriptIdentifier));
        var script = scriptElement?.InnerHtml.Split(["\n", "\r\n"], StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault(l => l.Contains(scriptIdentifier))?
            .Replace(scriptIdentifier, "")
            .Replace("undefined", "null")
            .Trim();

        if (script is null)
        {
            _logger.LogError("To my knowledge, all AH pages should have this json object");
            return ExtractResult.Fail;
        }

        var ahObject = DeserializeToAhObject(script);

        if (ahObject is null)
        {
            _logger.LogError("To my knowledge, all AH pages should have this json object");
            return ExtractResult.Fail;
        }

        var ahNutrition = ahObject?.product?.card?.meta?.nutritions?.FirstOrDefault(n => n.servingSize!.Contains("100"));

        if (ahNutrition is null)
        {
            _logger.LogInformation("This product has no nutrition info. Skipping");
            return ExtractResult.Success;
        }

        var servingSizeAndUnit = ExtractSizeAndUnitFrom(ahNutrition.servingSize);
        if (servingSizeAndUnit is null) return ExtractResult.Fail;

        var recommendedSize = ExtractSizeAndUnitFrom(ahNutrition.servingSizeDescription);

        var ahNutrients = ahNutrition.nutrients ?? [];

        var caloriesNutrient = ahNutrients.FirstOrDefault(n => n.type == NutrientType.Calories)?.value ?? "";
        var kcalRegex = new Regex("(?<kcal>\\d+) kcal");
        var kcalCapture = kcalRegex.Match(caloriesNutrient);
        var kcalGroup = kcalCapture.Groups["kcall"];
        var kcal = kcalGroup.Success ? double.Parse(kcalGroup.Value) : 0;

        var fatsNutrient = ahNutrients.FirstOrDefault(n => n.type == NutrientType.Fats)?.value ?? "";
        var fats = ExtractSizeAndUnitFrom(fatsNutrient);

        // TODO: write the rest...
        // var fatsNutrient = ahNutrients.FirstOrDefault(n => n.type == NutrientType.Fats)?.value ?? "";
        // var fats = ExtractSizeAndUnitFrom(fatsNutrient);

        var nutrition = new NutritionInfo
        {
            PerUnit = servingSizeAndUnit.Value.Unit,
            Per = Convert.ToInt32(servingSizeAndUnit.Value.Size),
            PortionRecommended = Convert.ToInt32(recommendedSize.GetValueOrDefault().Size),
            Calories = kcal,
            Fats = fats?.Size


        };

        throw new NotImplementedException();
    }

    private AhObject? DeserializeToAhObject(string script)
    {
        AhObject? ahObject = null;

        try
        {
            ahObject = JsonSerializer.Deserialize<AhObject>(script);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "To my knowledge, all AH pages should have this json object");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "The JSON is somehow different than the one I tested in the unit tests");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Something must have gone wrong that I haven't foreseen");
        }

        return ahObject;
    }

    private (double Size, Unit Unit)? ExtractSizeAndUnitFrom(string? text)
    {
        if (text is null) return null;
        var servingSizeAndUnit = text.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? [];
        if (servingSizeAndUnit.Length != 2)
        {
            _logger.LogError("Unknown serving size. Contains more parts than unit and size: {Value}", text);
            return null;
        }

        Unit? servingUnit = servingSizeAndUnit.Last().ToLower() switch
        {
            "gram" or "g" => Unit.Grams,
            "milliliters" => Unit.Milliliters,
            _ => null
        };

        if (!servingUnit.HasValue)
        {
            _logger.LogError("Unknown serving unit. {Value}", text);
            return null;
        }

        if (!double.TryParse(servingSizeAndUnit[0], out double servingSize))
        {
            _logger.LogError("Serving size is not a number (first part): {Value}", text);
            return null;
        }

        return (servingSize, servingUnit.Value);
    }

    private static class NutrientType
    {
        public const string Calories = "ENER-";
        public const string Fats = "FAT";
        public const string FatsSaturated = "FATAS";
        public const string FatsUnsaturated = "X_FUNS";
        public const string Carbs = "CHOAVL";
        public const string Sugars = "SUGAR-";
        public const string Fibers = "FIBTG";
        public const string Proteins = "PRO-";
        public const string Salts = "SALTEQ";
    }
}
