using AngleSharp.Dom;

namespace Scraper.ProductExtraction.PropertyExtractors.Ah;

public static class AhUtils
{
    public static IElement? QueryHeroSection(IElement? element)
    {
        return element?.QuerySelector("#start-of-content > div:first-child");
    }

    public static IElement? QueryDetailsSection(IElement? element)
    {
        return element?.QuerySelector("#start-of-content > div:nth-child(2)");
    }
}
