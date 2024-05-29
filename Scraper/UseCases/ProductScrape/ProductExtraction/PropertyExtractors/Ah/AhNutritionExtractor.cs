using AngleSharp.Dom;
using Scraper.UseCases.ProductScrape.ProductExtraction;
using Scraper.UseCases.ProductScrape.ProductExtraction.PropertyExtractors;
using System.Text.Json;
using Core.Data.Types;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Scraper.UseCases.ProductScrape.ProductExtraction.PropertyExtractors.Ah;

public class AhNutritionExtractor : IAhPropertyExtractor
{
    private string? _productTitle = null;
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

        _productTitle = ahObject.Title;

        var ahNutrition = ahObject?.product?.card?.meta?.nutritions?.FirstOrDefault(n => n.servingSize?.Contains("100") ?? false);

        if (ahNutrition is null)
        {
            _logger.LogInformation("Product {Title} has no nutrition info. Skipping", _productTitle);
            return ExtractResult.Success;
        }

        _logger.LogInformation("Nutrition found on product {Title}", _productTitle);

        var servingSizeAndUnit = ExtractSizeAndUnitFrom(ahNutrition.servingSize);
        if (servingSizeAndUnit is null) return ExtractResult.Fail;

        var recommendedSize = ExtractSizeAndUnitFrom(ahNutrition.servingSizeDescription);

        var ahNutrients = ahNutrition.nutrients ?? [];

        var caloriesNutrient = ahNutrients.FirstOrDefault(n => n.type == NutrientType.Calories)?.value ?? "";
        var groupKey = "kcal";
        var kcalRegex = new Regex($"(?<{groupKey}>\\d+) kcal");
        var kcalCapture = kcalRegex.Match(caloriesNutrient);
        var kcalGroup = kcalCapture.Groups[groupKey];
        var kcal = kcalGroup.Success ? double.Parse(kcalGroup.Value) : 0;

        var nutrition = new NutritionInfo
        {
            PerUnit = servingSizeAndUnit.Value.Unit,
            Per = Convert.ToInt32(servingSizeAndUnit.Value.Size),
            PortionRecommended = Convert.ToInt32(recommendedSize.GetValueOrDefault().Size),
            Calories = kcal,
            Fats = ExtractSizeOf(NutrientType.Fats, ahNutrients),
            FatsUnsaturated = ExtractSizeOf(NutrientType.FatsUnsaturated, ahNutrients),
            FatsSaturated = ExtractSizeOf(NutrientType.FatsSaturated, ahNutrients),
            Carbs = ExtractSizeOf(NutrientType.Carbs, ahNutrients),
            Sugars = ExtractSizeOf(NutrientType.Sugars, ahNutrients),
            Proteines = ExtractSizeOf(NutrientType.Proteins, ahNutrients),
            Fibres = ExtractSizeOf(NutrientType.Fibres, ahNutrients),
            Salts = ExtractSizeOf(NutrientType.Salts, ahNutrients),
            PreparationState = ExtractPreparationState(ahNutrition.preparationState)
        };

        builder.AddNutritionInfo(nutrition);
        return ExtractResult.Success;
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

    private double? ExtractSizeOf(string type, Nutrient[]? ahNutrients)
    {
        var nutrient = ahNutrients?.FirstOrDefault(n => n.type == type)?.value ?? "";
        return ExtractSizeAndUnitFrom(nutrient)?.Size;
    }

    private (double Size, Unit Unit)? ExtractSizeAndUnitFrom(string? text)
    {
        if (text is null) return null;
        var servingSizeAndUnit = text.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? [];
        if (servingSizeAndUnit.Length != 2)
        {
            _logger.LogError("{Title}: Unknown serving size. Contains more parts than unit and size: {Value}", _productTitle, text);
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
            _logger.LogError("{Title}: Unknown serving unit. {Value}", _productTitle, text);
            return null;
        }

        if (!double.TryParse(servingSizeAndUnit[0], CultureInfo.InvariantCulture, out double servingSize))
        {
            _logger.LogError("{Title}: Serving size is not a number (first part): {Value}", _productTitle, text);
            return null;
        }

        return (servingSize, servingUnit.Value);
    }

    private PreparationState? ExtractPreparationState(string? preparationState)
    {
        PreparationState? state = preparationState?.ToLower() switch
        {
            "onbereide" => PreparationState.Unprepared,
            "bereide" => PreparationState.Prepared,
            "" => null,
            _ => null
        };

        if (state is null)
        {
            _logger.LogInformation("{Title}: Product has no preparation state", _productTitle);
        }

        return state;
    }

    private static class NutrientType
    {
        public const string Calories = "ENER-";
        public const string Fats = "FAT";
        public const string FatsSaturated = "FASAT";
        public const string FatsUnsaturated = "X_FUNS";
        public const string Carbs = "CHOAVL";
        public const string Sugars = "SUGAR-";
        public const string Fibres = "FIBTG";
        public const string Proteins = "PRO-";
        public const string Salts = "SALTEQ";
    }
}
