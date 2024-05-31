namespace Core.Data.Types;

public class Ingredient
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Product> Products { get; set; } = [];
}