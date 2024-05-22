using AngleSharp.Dom;
using Scraper.UseCases.ProductScrape.ProductExtraction;
using Scraper.UseCases.ProductScrape.ProductExtraction.PropertyExtractors;

namespace Scraper.UseCases.ProductScrape.ProductExtraction.PropertyExtractors.Ah;

public class AhNutriscorePropertyExtractor : IAhPropertyExtractor
{
    private readonly ILogger<AhNutriscorePropertyExtractor> _logger;

    public AhNutriscorePropertyExtractor(ILogger<AhNutriscorePropertyExtractor> logger)
    {
        _logger = logger;
    }

    public ExtractResult Extract(IDocument document, ProductBuilder builder)
    {
        var nutriscoreElement = AhUtils
            .QueryHeroSection(document.Body)?
            .QuerySelector(".nutriscore_root__clyII svg title");

        var title = nutriscoreElement?.TextContent;

        if (string.IsNullOrWhiteSpace(title))
        {
            _logger.LogError("No nutriscore element found.");
            return ExtractResult.NotFound;
        }

        var nutriscore = title.Split(' ').Last().ToUpper();

        builder.Nutriscore(nutriscore);
        return ExtractResult.Success;
    }
}