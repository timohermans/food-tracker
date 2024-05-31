namespace Core.Data.Types;

public class Product : IAuditable
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public decimal Price { get; set; }
    public string? UnitSize { get; set; }
    public Nutriscore? Nutriscore { get; set; }
    /// <summary>
    /// Sort of the main selling points of the product. This is an HTML string!
    /// </summary>
    public string? Summary { get; set; }
    public ICollection<Ingredient>? Ingredients { get; set; }
    public int? NutritionInfoId { get; set; }
    public NutritionInfo? NutritionInfo { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
