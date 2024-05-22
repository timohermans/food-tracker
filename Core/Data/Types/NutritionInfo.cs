namespace Core.Data.Types;

public enum Unit
{
    Grams,
    Milliliters
}

public class NutritionInfo
{
    public int Id { get; set; }
    public int Per { get; set; } // "100"
    public int? PortionRecommended { get; set; }
    public Unit PerUnit { get; set; } // "grams"
    public double Calories { get; set; }
    public double? Carbs { get; set; }
    public double? Sugars { get; set; }
    public double? Proteines { get; set; }
    public double? Fats { get; set; }
    public double? FatsSaturated { get; set; }
    public double? FatsUnsaturated { get; set; }
    public double? Fibres { get; set; }
    public double? Salts { get; set; }
}
