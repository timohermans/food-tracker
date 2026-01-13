using AngleSharp.Dom;
using Core.Data.Types;

namespace Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors.Ah.NutritionEnrichers;

public interface IAhNutritionEnricher
{
    (NutritionInfo NutritionInfo, string? Error) Enrich(NutritionInfo nutritionInfo, IDocument document);
}