using Scraper.ProductExtraction.PropertyExtractors;

namespace Scraper;

public static class ServiceWorkerExtensions
{
    public static void AddPropertyExtractors(this IServiceCollection services)
    {
        typeof(ServiceWorkerExtensions)
            .Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IProductPropertyExtractor)))
            .ToList()
            .ForEach(t => services.AddTransient(typeof(IProductPropertyExtractor), t));
    }

    public static void AddUseCases(this IServiceCollection services)
    {
        typeof(ServiceWorkerExtensions)
            .Assembly
            .GetTypes()
            .Where(t => t.Name.EndsWith("UseCase"))
            .ToList()
            .ForEach(t => services.AddTransient(t));
    }
}