using Microsoft.Extensions.Logging;

namespace Tests;

public static class Helper
{
    public static ILogger<T> GetLogger<T>()
    {
        using var logFactory = LoggerFactory.Create(builder => builder.AddConsole());
        return logFactory.CreateLogger<T>();
    }

    public static ILogger GetLogger(Type t)
    {
        using var logFactory = LoggerFactory.Create(builder => builder.AddConsole());
        return logFactory.CreateLogger(t);
    }

    public static object? GetPropertyValue(object? src, string propName)
    {
        ArgumentNullException.ThrowIfNull(propName);
        if (src is null) return null;

        if (propName.Contains('.'))
        {
            var temp = propName.Split(['.'], 2);
            return GetPropertyValue(GetPropertyValue(src, temp[0]), temp[1]);
        }

        var prop = src.GetType().GetProperty(propName);
        return prop != null ? prop.GetValue(src, null) : null;
    }
}