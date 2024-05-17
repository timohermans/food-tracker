namespace Core.Data.Types;

public class Product : IAuditable
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public decimal Price { get; set; }
    public int UnitSize { get; set; }
    public string? Nutriscore { get; set; }
    public string? Summary { get; set; }
    public string[]? SellingPoints {get; set;}
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
