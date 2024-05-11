
namespace Core.Data.Types;

public class ProductScrapeJob : IAuditable
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public string? Content { get; set; }
    public bool IsContentFetched => !string.IsNullOrEmpty(Content);
    public bool? HasNutritionInfo { get; set; } = null;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
