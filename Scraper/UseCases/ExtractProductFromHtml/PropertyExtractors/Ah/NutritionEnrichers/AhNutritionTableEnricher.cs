using AngleSharp.Dom;
using Core.Data.Types;
using Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors.Shared;

namespace Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors.Ah.NutritionEnrichers;

public class AhNutritionTableEnricher(ILogger<AhNutritionTableEnricher> logger) : IAhNutritionEnricher
{
    private Dictionary<string, Action<NutritionInfo, string>> _nutritionExtractors = new()
    {
        { "energie", (n, v) => n.Calories = NutritionTableParser.ParseKiloCaloriesFrom(v) },
        { "vetten", (n, v) => n.Fats = PortionAndUnitParser.Parse(v).Amount },
        { "vet", (n, v) => n.Fats = PortionAndUnitParser.Parse(v).Amount },
        { "waarvan verzadigde vetzuren", (n, v) => n.FatsSaturated = PortionAndUnitParser.Parse(v).Amount },
        { "waarvan verzadigd", (n, v) => n.FatsSaturated = PortionAndUnitParser.Parse(v).Amount },
        {
            "waarvan enkelvoudig onverzadigde vetzuren",
            (n, v) => n.FatsUnsaturated = PortionAndUnitParser.Parse(v).Amount
        },
        { "waarvan onverzadigd", (n, v) => n.FatsUnsaturated = PortionAndUnitParser.Parse(v).Amount },
        { "koolhydraten", (n, v) => n.Carbs = PortionAndUnitParser.Parse(v).Amount },
        { "waarvan suikers", (n, v) => n.Sugars = PortionAndUnitParser.Parse(v).Amount },
        { "eiwitten", (n, v) => n.Proteines = PortionAndUnitParser.Parse(v).Amount },
        { "vezels", (n, v) => n.Fibres = PortionAndUnitParser.Parse(v).Amount },
        { "zout", (n, v) => n.Salts = PortionAndUnitParser.Parse(v).Amount },
        { "voedingsvezel", (n, v) => n.Fibres = PortionAndUnitParser.Parse(v).Amount },
    };

    public (NutritionInfo NutritionInfo, string? Error) Enrich(NutritionInfo nutritionInfo, IDocument document)
    {
        var tables = document.QuerySelectorAll("table");
        var nutritionTable = tables
            .FirstOrDefault(e => e.ClassList.Any(c => c.StartsWith("product-info-nutrition")));

        if (nutritionTable == null)
        {
            return (nutritionInfo, "Nutrition table not found");
        }

        // first loop through the table head to find the per unit index
        var columnHeaders = nutritionTable.QuerySelectorAll("thead > tr > th").ToList();
        var perUnitIndex = columnHeaders.FindIndex(th => th.TextContent.Contains("100"));

        var rows = nutritionTable.QuerySelectorAll("tbody > tr").ToList();

        foreach (var row in rows)
        {
            var cells = row.QuerySelectorAll("td").ToList();
            var nutritionName = cells[0].TextContent.Trim().ToLower();
            var valueText = cells[perUnitIndex].TextContent.Trim().ToLower();

            if (!_nutritionExtractors.TryGetValue(nutritionName, out var extractor))
            {
                logger.LogWarning($"No extractor found for nutrition: {nutritionName}");
                continue;
            }

            extractor(nutritionInfo, valueText);
        }

        return (nutritionInfo, null);
    }
}