using AngleSharp.Dom;

namespace Scraper.ProductExtraction.PropertyExtractors.Ah;

public class AhUnitSizePropertyExtractor : IAhPropertyExtractor
{
    private readonly ILogger<AhUnitSizePropertyExtractor> _logger;

    public AhUnitSizePropertyExtractor(ILogger<AhUnitSizePropertyExtractor> logger)
    {
        _logger = logger;
    }

    public ExtractResult Extract(IDocument document, ProductBuilder builder)
    {
        var unitSizeElement = AhUtils
            .QueryHeroSection(document.Body)?
            .QuerySelector("[data-testhook=\"product-unit-size\"]");
        if (unitSizeElement is null)
        {
            _logger.LogError("No unit size element found.");
            return ExtractResult.NotFound;
        }

        builder.UnitSize(unitSizeElement.TextContent);
        return ExtractResult.Success;
    }
}