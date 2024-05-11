using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Web;

public static class WebConfigurationExtensions
{
    public static IServiceCollection AddEndpointsFrom(this IServiceCollection services, Assembly assembly)
    {
        typeof(Program).Assembly
            .GetTypes()
            .Where(t => !t.IsInterface && t.IsAssignableTo(typeof(IEndpoint)))
            .Select(t => ServiceDescriptor.Transient(typeof(IEndpoint), t))
            .ToList()
            .ForEach(services.TryAddEnumerable);
        return services;
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.Services.GetRequiredService<IEnumerable<IEndpoint>>()
            .ToList()
            .ForEach(e => e.Configure(app));
        return app;
    }

    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        typeof(WebConfigurationExtensions)
            .Assembly
            .GetTypes()
            .Where(t => t.Name.EndsWith("UseCase"))
            .ToList()
            .ForEach(t => services.AddTransient(t));

        return services;
    }
}
