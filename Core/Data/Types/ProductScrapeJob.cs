namespace Core.Data.Types;

public class ProductScrapeJob : IAuditable
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public string? Content { get; set; }
    public bool IsContentFetched => !string.IsNullOrEmpty(Content);
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Product? Product { get; set; }

    public override string ToString()
    {
        if (Url.Contains('/'))
        {
            return Url.Split('/', StringSplitOptions.RemoveEmptyEntries).Last();
        }

        return Url;
    }
}