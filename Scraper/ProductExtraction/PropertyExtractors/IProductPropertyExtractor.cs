using AngleSharp.Dom;

namespace Scraper.ProductExtraction.PropertyExtractors;

public interface IProductPropertyExtractor
{
    public ExtractResult Extract(IDocument element, ProductBuilder builder);
}
