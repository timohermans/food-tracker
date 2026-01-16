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

    public AhNutritionExtractor(ILogger<AhNutritionExtractor> logger)
    {
        _logger = logger;
    }

    public ExtractResult Extract(IDocument document, ProductBuilder builder)
    {

        var nutritionInfo = new NutritionInfo();

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

        var preparationStateText = "waarden gelden voor";
        var preparationStateRegex = new Regex(@$"{preparationStateText} het (\w+) product", RegexOptions.Compiled);
        var preparationStateContent = document.QuerySelectorAll("[data-testhook=\"pdp-info-content\"] p")
            .FirstOrDefault(span =>
                span.TextContent.Trim().Contains(preparationStateText, StringComparison.OrdinalIgnoreCase))
            ?.TextContent.Trim().ToLower();
        if (preparationStateContent is not null)
        {
            var match = preparationStateRegex.Match(preparationStateContent);
            if (match.Success)
            {
                var prepState = match.Groups[1].Value;
                nutritionInfo.PreparationState = ExtractPreparationState(prepState);
            }
        }

        builder.AddNutritionInfo(nutritionInfo);
        return ExtractResult.Success;
    }

    private PreparationState? ExtractPreparationState(string? preparationState)
    {
        PreparationState? state = preparationState?.ToLower() switch
        {
            "onbereide" => PreparationState.Unprepared,
            "bereide" => PreparationState.Prepared,
            _ => null
        };

        if (state is null)
        {
            _logger.LogInformation("{Title}: Product has no preparation state", _productTitle);
        }

        return state;
    }
}