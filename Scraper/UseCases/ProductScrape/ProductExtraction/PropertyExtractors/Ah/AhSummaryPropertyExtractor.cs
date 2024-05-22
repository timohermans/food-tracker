using AngleSharp.Dom;
using Scraper.UseCases.ProductScrape.ProductExtraction;
using Scraper.UseCases.ProductScrape.ProductExtraction.PropertyExtractors;

namespace Scraper.UseCases.ProductScrape.ProductExtraction.PropertyExtractors.Ah;

public class AhSummaryPropertyExtractor : IAhPropertyExtractor
{
    private readonly ILogger<AhSummaryPropertyExtractor> _logger;

    public AhSummaryPropertyExtractor(ILogger<AhSummaryPropertyExtractor> logger)
    {
        _logger = logger;
    }

    public ExtractResult Extract(IDocument document, ProductBuilder builder)
    {
        var summaryElement = AhUtils
            .QueryHeroSection(document.Body)?
            .QuerySelector("[data-testhook=\"product-summary\"]");
        if (summaryElement is null)
        {
            _logger.LogError("No summary element found.");
            return ExtractResult.NotFound;
        }

        builder.Summary(summaryElement.InnerHtml);
        return ExtractResult.Success;
    }
}