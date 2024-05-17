using Microsoft.Extensions.Logging;

namespace Tests;

public static class Helper
{
    public static ILogger<T> GetLogger<T>()
    {
        using var logFactory = LoggerFactory.Create(builder => builder.AddConsole());
        return logFactory.CreateLogger<T>();
    }
}