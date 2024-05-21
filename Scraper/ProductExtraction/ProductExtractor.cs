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

        var product = builder.Build();

        // TODO: ProductPersister -> Denk aan ingredients die duplicate in DB gaan komen als ik niets doe! ofja komen, erroren :joy:

        return new ProductSuccess(product);
    }
}