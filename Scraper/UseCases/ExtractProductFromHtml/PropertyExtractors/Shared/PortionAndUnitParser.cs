using System.Globalization;
using System.Text.RegularExpressions;
using Core.Data.Types;

namespace Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors.Shared;

public static class PortionAndUnitParser
{
    public static (double Amount, Unit Unit) Parse(string rawText)
    {
        var regex = new Regex(
            @"(?i)\b(\d+(?:[.,]\d+)?)\s*(ml|g|gram|milliliter|ml)\b",
            RegexOptions.Compiled);

        var match = regex.Match(rawText);

        if (!match.Success)
        {
            throw new InvalidOperationException(
                "No amount + unit found even though \"Portiegrootte\" was found. The text was: " + rawText);
        }

        var amountText = match.Groups[1].Value.Replace(',', '.');
        var unitRaw = match.Groups[2].Value;

        var portionAmount = double.Parse(
            amountText,
            NumberStyles.Number,
            CultureInfo.InvariantCulture);
        
        var unit = unitRaw.ToLower().Trim() switch
        {
            "gram" or "g" => Unit.Grams,
            "milliliter" or "ml" => Unit.Milliliters,
            _ => throw new NotImplementedException($"Unknown unit type: {unitRaw}")
        };
        
        return (portionAmount, unit);
    }
}