using Core.Data.Types;
using System.Globalization;

namespace Scraper.UseCases.ExtractProductFromHtml;

public class ProductBuilder
{
    private string? _title;
    private decimal? _price;
    private string? _unitSize;
    private Nutriscore _nutriscore;
    private string? _summary;
    private List<Ingredient>? _ingredients;
    private NutritionInfo? _nutritionInfo;

    public ProductBuilder()
    {
    }

    public ProductBuilder Title(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        _title = Clean(title);
        return this;
    }

    public ProductBuilder Price(string priceStr)
    {
        var cleanedPriceStr = Clean(priceStr);
        if (!decimal.TryParse(cleanedPriceStr, CultureInfo.InvariantCulture, out var price))
        {
            throw new ArgumentException("invalid price", nameof(priceStr));
        }

        _price = price;
        return this;
    }

    public ProductBuilder UnitSize(string unitSize)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(unitSize);
        _unitSize = Clean(unitSize);
        return this;
    }

    public ProductBuilder Nutriscore(string nutriscoreStr)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nutriscoreStr);
        _nutriscore = Enum.Parse<Nutriscore>(Clean(nutriscoreStr));
        return this;
    }

    public ProductBuilder Summary(string summaryStr)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(summaryStr);
        _summary = Clean(summaryStr);
        return this;
    }

    public ProductBuilder Ingredients(IEnumerable<string> ingredients)
    {
        _ingredients = ingredients.Select(i => new Ingredient { Name = i }).ToList();
        return this;
    }

    public ProductBuilder AddNutritionInfo(NutritionInfo nutrition)
    {
        _nutritionInfo = nutrition;
        return this;
    }

    public Product Build()
    {
        return new Product
        {
            Title = _title ?? throw new ArgumentNullException(nameof(Title)),
            Price = _price ?? throw new ArgumentNullException(nameof(Price)),
            UnitSize = _unitSize,
            Nutriscore = _nutriscore,
            Summary = _summary,
            Ingredients = _ingredients,
            NutritionInfo = _nutritionInfo
        };
    }

    public static string Clean(string content)
    {
        return string.Join(" ", content
            .Split(["\n", "\r\n"], StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim()));
    }
}