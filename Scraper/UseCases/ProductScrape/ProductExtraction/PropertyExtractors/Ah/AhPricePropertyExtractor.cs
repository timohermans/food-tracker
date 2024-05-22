using AngleSharp.Dom;
using Scraper.UseCases.ProductScrape.ProductExtraction;
using Scraper.UseCases.ProductScrape.ProductExtraction.PropertyExtractors;

namespace Scraper.UseCases.ProductScrape.ProductExtraction.PropertyExtractors.Ah;

public class AhPricePropertyExtractor : IAhPropertyExtractor
{
    private readonly ILogger<AhPricePropertyExtractor> _logger;

    public AhPricePropertyExtractor(ILogger<AhPricePropertyExtractor> logger)
    {
        _logger = logger;
    }

    public ExtractResult Extract(IDocument document, ProductBuilder builder)
    {
        var priceElement = AhUtils
            .QueryHeroSection(document.Body)?
            .QuerySelector("[data-testhook=\"price-amount\"]");
        if (priceElement is null)
        {
            _logger.LogWarning("No price found.");
            return ExtractResult.NotFound;
        }

        builder.Price(priceElement.TextContent);
        return ExtractResult.Success;
    }
}