namespace Core.Data.Types;

public class FoodEntry
{
    public int Id { get; set; }
    public double Amount { get; set; }
    public double Per { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public double Calories
    {
        get
        {
            if (Product?.NutritionInfo is null) return 0;
            var nutritionInfo = Product.NutritionInfo;
            return nutritionInfo.Calories / nutritionInfo.Per * Amount * Per;
        }
    }
}