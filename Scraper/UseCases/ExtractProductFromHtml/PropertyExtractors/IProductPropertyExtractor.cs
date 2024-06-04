using AngleSharp.Dom;

namespace Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors;

public interface IProductPropertyExtractor
{
    public ExtractResult Extract(IDocument element, ProductBuilder builder);
}
