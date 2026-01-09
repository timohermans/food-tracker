using AngleSharp.Dom;

namespace Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors.Ah;

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
            .QuerySelector("[data-testid=\"product-summary\"]");
        if (summaryElement is null)
        {
            _logger.LogError("No summary element found.");
            return ExtractResult.NotFound;
        }

        builder.Summary(summaryElement.InnerHtml);
        return ExtractResult.Success;
    }
}