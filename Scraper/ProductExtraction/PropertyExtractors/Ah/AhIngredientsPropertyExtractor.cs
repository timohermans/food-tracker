using AngleSharp.Dom;

namespace Scraper.ProductExtraction.PropertyExtractors.Ah;

public class AhIngredientsPropertyExtractor : IAhPropertyExtractor
{
    public ExtractResult Extract(IDocument element, ProductBuilder builder)
    {
        var ingredientsElement = AhUtils.
            QueryDetailsSection(element.Body)?
            .QuerySelector();
    }
}