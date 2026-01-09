using AngleSharp.Dom;

namespace Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors.Ah;

public class AhTitlePropertyExtractor : IAhPropertyExtractor
{
    private readonly ILogger<AhTitlePropertyExtractor> _logger;

    public AhTitlePropertyExtractor(ILogger<AhTitlePropertyExtractor> logger)
    {
        _logger = logger;
    }

    public ExtractResult Extract(IDocument document, ProductBuilder builder)
    {
        var titleElement = AhUtils.QueryHeroSection(document.Body)?.QuerySelector("h1");

        if (titleElement is null)
        {
            _logger.LogError("No h1 element found. Something must be wrong with the content");
            return ExtractResult.Fail;
        }

        builder.Title(titleElement.TextContent.Trim());
        return ExtractResult.Success;
    }
}