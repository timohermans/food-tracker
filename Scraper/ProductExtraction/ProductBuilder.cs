using Core.Data.Types;

namespace Scraper.ProductExtraction;

public class ProductBuilder
{
    private string? _title;
    public ProductBuilder() { }

    public ProductBuilder Title(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        _title = Clean(title);
        return this;
    }

    public Product Build()
    {
        return new Product
        {
            Title = _title ?? throw new ArgumentNullException(nameof(Title))
        };
    }

    private string Clean(string content)
    {
        return string.Join(" ", content
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim()));
    }
}

