using System.Text.RegularExpressions;

namespace Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors.Shared;

public static class NutritionTableParser
{
    public static double ParseKiloCaloriesFrom(string caloriesText)
    {
        if (double.TryParse(caloriesText, out var simpleCalories))
        {
            return simpleCalories;
        }

        var groupKey = "kcal";
        var kcalRegex = new Regex($"(?<{groupKey}>\\d+(?:\\.\\d+)?) kcal");
        var kcalCapture = kcalRegex.Match(caloriesText);
        var kcalGroup = kcalCapture.Groups[groupKey];
        var kcal = kcalGroup.Success ? double.Parse(kcalGroup.Value) : 0;
        return kcal;
    }
}