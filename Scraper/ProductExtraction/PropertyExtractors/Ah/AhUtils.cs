using AngleSharp.Dom;

namespace Scraper.ProductExtraction.PropertyExtractors.Ah;

public static class AhUtils
{
    public static IElement? QueryHeroSection(IElement? element) {
        return element?.QuerySelector("#start-of-content div:first-child");
    }
}
