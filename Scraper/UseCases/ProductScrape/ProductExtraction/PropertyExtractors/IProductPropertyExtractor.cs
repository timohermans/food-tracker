using AngleSharp.Dom;
using Scraper.UseCases.ProductScrape.ProductExtraction;

namespace Scraper.UseCases.ProductScrape.ProductExtraction.PropertyExtractors;

public interface IProductPropertyExtractor
{
    public ExtractResult Extract(IDocument element, ProductBuilder builder);
}
