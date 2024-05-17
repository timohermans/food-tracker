namespace Core.Data.Types;

public class ProductBuilder {
    private string? _title;
    public ProductBuilder() {}

    public ProductBuilder Title(string title) {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        _title = title;
        return this;
    }

    public Product Build() {
        return new Product {
            Title = _title ?? throw new ArgumentNullException(nameof(Title))
        };
    }
}

public class Product
{
    public required string Title { get; set; }
    public decimal Price { get; set; }
    public int UnitSize { get; set; }
    public string? Nutriscore { get; set; }
    public string? Summary { get; set; }
    public string[]? SellingPoints {get; set;}


}
