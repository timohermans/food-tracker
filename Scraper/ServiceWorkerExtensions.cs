namespace Scraper;

public static class ServiceWorkerExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        typeof(ServiceWorkerExtensions)
            .Assembly
            .GetTypes()
            .Where(t => t.Name.EndsWith("UseCase"))
            .ToList()
            .ForEach(t => services.AddTransient(t));

        return services;
    }
}
