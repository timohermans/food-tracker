using AngleSharp.Dom;
using System.Text.Json;
using Core.Data.Types;
using System.Text.RegularExpressions;
using System.Globalization;
using Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors.Shared;

namespace Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors.Ah;

public class AhNutritionExtractor : IAhPropertyExtractor
{
    private string? _productTitle = null;
    private readonly ILogger<AhNutritionExtractor> _logger;

    private Dictionary<string, Action<NutritionInfo, string>> _nutritionExtractors = new()
    {
        { "energie", (n, v) => n.Calories = NutritionTableParser.ParseKiloCaloriesFrom(v) },
        { "vetten", (n, v) => n.Fats = PortionAndUnitParser.Parse(v).Amount },
        { "vet", (n, v) => n.Fats = PortionAndUnitParser.Parse(v).Amount },
        { "waarvan verzadigde vetzuren", (n, v) => n.FatsSaturated = PortionAndUnitParser.Parse(v).Amount },
        { "waarvan verzadigd", (n, v) => n.FatsSaturated = PortionAndUnitParser.Parse(v).Amount },
        { "waarvan enkelvoudig onverzadigde vetzuren", (n, v) => n.FatsUnsaturated = PortionAndUnitParser.Parse(v).Amount },
        { "waarvan onverzadigd", (n, v) => n.FatsUnsaturated = PortionAndUnitParser.Parse(v).Amount },
        { "koolhydraten", (n, v) => n.Carbs = PortionAndUnitParser.Parse(v).Amount },
        { "waarvan suikers", (n, v) => n.Sugars = PortionAndUnitParser.Parse(v).Amount },
        { "eiwitten", (n, v) => n.Proteines = PortionAndUnitParser.Parse(v).Amount },
        { "vezels", (n, v) => n.Fibres = PortionAndUnitParser.Parse(v).Amount },
        { "zout", (n, v) => n.Salts = PortionAndUnitParser.Parse(v).Amount },
        { "voedingsvezel", (n, v) => n.Fibres = PortionAndUnitParser.Parse(v).Amount },
    };

    public AhNutritionExtractor(ILogger<AhNutritionExtractor> logger)
    {
        _logger = logger;
    }

    public ExtractResult Extract(IDocument document, ProductBuilder builder)
    {
        var tables = document.QuerySelectorAll("table");
        var nutritionTable = tables
            .FirstOrDefault(e => e.ClassList.Any(c => c.StartsWith("product-info-nutrition")));

        if (nutritionTable == null)
        {
            return ExtractResult.NotFound;
        }

        // first loop through the table head to find the per unit index
        var columnHeaders = nutritionTable.QuerySelectorAll("thead > tr > th").ToList();
        var perUnitIndex = columnHeaders.FindIndex(th => th.TextContent.Contains("100"));

        var rows = nutritionTable.QuerySelectorAll("tbody > tr").ToList();

        var nutritionInfo = new NutritionInfo();
        foreach (var row in rows)
        {
            var cells = row.QuerySelectorAll("td").ToList();
            var nutritionName = cells[0].TextContent.Trim().ToLower();
            var valueText = cells[perUnitIndex].TextContent.Trim().ToLower();

            if (!_nutritionExtractors.TryGetValue(nutritionName, out var extractor))
            {
                _logger.LogWarning($"No extractor found for nutrition: {nutritionName}");
                continue;
            }

            extractor(nutritionInfo, valueText);
        }

        _logger.LogInformation("Nutrition found on product {Title}", _productTitle);

        var servingSizeAndUnit = PortionAndUnitParser.Parse(columnHeaders[perUnitIndex].TextContent
            .Replace("Per ", "", StringComparison.InvariantCultureIgnoreCase));
        nutritionInfo.Per = servingSizeAndUnit.Amount;
        nutritionInfo.PerUnit = servingSizeAndUnit.Unit;

        var portionSizeRaw = document.QuerySelectorAll("[data-testhook=\"pdp-info-content\"] span")
            .FirstOrDefault(span =>
                span.FirstElementChild?.TextContent.Trim()
                    .StartsWith("Portiegrootte:", StringComparison.OrdinalIgnoreCase) ?? false)
            ?.LastElementChild?.TextContent.Trim();
        if (portionSizeRaw is not null)
        {
            var (portionAmount, _) = PortionAndUnitParser.Parse(portionSizeRaw);
            nutritionInfo.PortionRecommended = portionAmount;
        }
        


        // var ahNutrients = ahNutrition.nutrients ?? [];

        // var caloriesNutrient = ahNutrients.FirstOrDefault(n => n.type == NutrientType.Calories)?.value ?? "";

        // var nutrition = new NutritionInfo
        // {
        //     PerUnit = servingSizeAndUnit.Value.Unit,
        //     Per = Convert.ToInt32(servingSizeAndUnit.Value.Size),
        //     PortionRecommended = Convert.ToInt32(recommendedSize.GetValueOrDefault().Size),
        //     Calories = kcal,
        //     Fats = ExtractSizeOf(NutrientType.Fats, ahNutrients),
        //     FatsUnsaturated = ExtractSizeOf(NutrientType.FatsUnsaturated, ahNutrients),
        //     FatsSaturated = ExtractSizeOf(NutrientType.FatsSaturated, ahNutrients),
        //     Carbs = ExtractSizeOf(NutrientType.Carbs, ahNutrients),
        //     Sugars = ExtractSizeOf(NutrientType.Sugars, ahNutrients),
        //     Proteines = ExtractSizeOf(NutrientType.Proteins, ahNutrients),
        //     Fibres = ExtractSizeOf(NutrientType.Fibres, ahNutrients),
        //     Salts = ExtractSizeOf(NutrientType.Salts, ahNutrients),
        //     PreparationState = ExtractPreparationState(ahNutrition.preparationState)
        // };

        builder.AddNutritionInfo(nutritionInfo);
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
}