using AngleSharp.Dom;
using System.Text.Json;

namespace Scraper.ProductExtraction.PropertyExtractors.Ah;

public class AhNutritionExtractor : IAhPropertyExtractor
{
    private readonly ILogger<AhNutritionExtractor> _logger;

    public AhNutritionExtractor(ILogger<AhNutritionExtractor> logger)
    {
        _logger = logger;
    }

    public ExtractResult Extract(IDocument element, ProductBuilder builder)
    {
        var scriptIdentifier = "window.__INITIAL_STATE__ =";
        var scriptElement = element.Scripts.FirstOrDefault(s => s.InnerHtml.Contains(scriptIdentifier));
        var script = scriptElement?.InnerHtml.Split(["\n", "\r\n"], StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault(l => l.Contains(scriptIdentifier))?
            .Replace(scriptIdentifier, "")
            .Replace("undefined", "null")
            .Trim();

        if (script is null)
        {
            _logger.LogError("To my knowledge, all AH pages should have this json object");
            return ExtractResult.Fail;
        }

        var ahObject = JsonSerializer.Deserialize<AhObject>(script);

        throw new NotImplementedException();
    }
}
