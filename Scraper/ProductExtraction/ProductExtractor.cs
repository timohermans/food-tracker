using AngleSharp;
using Scraper.ProductExtraction.PropertyExtractors;

namespace Scraper.ProductExtraction;

public class ProductExtractor
{
    private readonly ILogger<ProductExtractor> _logger;

    public ProductExtractor(ILogger<ProductExtractor> logger)
    {
        _logger = logger;
    }

    public async Task<ProductExtractionResult> ExtractAsync(string content,
        IEnumerable<IProductPropertyExtractor> propertyExtractors)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);

        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(content));

        var builder = new ProductBuilder();

        foreach (var extractor in propertyExtractors)
        {
            var result = extractor.Extract(document, builder);
            if (result == ExtractResult.Fail)
            {
                _logger.LogError("Failed to extract product information");
                return new ProductFailResult("Product extraction failed, because of property extractor: " +
                                             extractor.GetType().Name);
            }
        }

        return new ProductSuccess(builder.Build());
        // article#start-of-content > :first-child()
        // Title: h1
        // Price: data-testhook="product-card" > data-testhook="price-amount"
        // UnitSize: data-testhook="product-card" > data-testhook="product-unit-size"
        // Nutriscore: data-testhook="product-card" > class="sticker-icon svg svg--svg_nutriscore-c" (opt) > svg_nutriscore_x
        // Summary: data-testhook="product-card" > data-testhook="product-summary" > innerText
        // SellingPoints[]: data-testhook="product-card" > data-testhook="product-properties" > svg > title

        // article#start-of-content > :nth-child(2)
        // geen property: data-testhook="product-info-description" -> ProductInfo
        // ProductInfo.Descriptions[]: ul.product-info-description_list__IjksY > li
        // ProductInfo.Summary: h4["Extra informatie"] + data-testhook="product-summary" > innerText
        // ProductInfo.PortionSize: dl.product-info-definition-list_root__8YeaY > span.first > innerText (remove "Portiegrootte: ")
        // ProductInfo.PortionAmount: dl.product-info-definition-list_root__8YeaY > span.nth(2) > innerText (remove "Aantal porties: ")
        // ProductInfo.Characteristics: ul.product-info-icons_root__MRB83 > li > innerText ("Veganistisch")
        // ProductInfo.Ingredients: div["Ingrediënten"] > p.first
        // ProductInfo.Allergens (Propability, Allergens): h4["Allergie-informatie"] + dl > (dt, dl)

        // ProductInfo.NutritionInfo: .product-info-nutrition_table__Q3m80
        // ProductInfo.NutritionInfo.Per: tr.first > th:not(:first)
        // ProductInfo.NutritionInfo.Energy: tr.nth(2)
        // ProductInfo.NutritionInfo.Fat: tr.nth(3)
        // ProductInfo.NutritionInfo.FatSaturated: tr.nth(4)
        // ProductInfo.NutritionInfo.Carbs: tr.nth(5)
        // ProductInfo.NutritionInfo.Sugar: tr.nth(6)
        // ProductInfo.NutritionInfo.Fiber: tr.nth(7)
        // ProductInfo.NutritionInfo.Protein: tr.nth(8)
        // ProductInfo.NutritionInfo.Salt: tr.nth(9)

        // ProductInfo.Usage: .product-info-usage_root__y0B+w
        // ProductInfo.Storage: .product-info-storage_root__lL8pe
    }
}