using Microsoft.Extensions.Logging;

namespace Tests;

public static class TestLogger
{
    public static ILogger<T> Create<T>()
    {
        var logger = new NUnitLogger<T>();
        return logger;
    }

    class NUnitLogger<T> : ILogger<T>, IDisposable
    {
        private readonly Action<string> output = Console.WriteLine;

        public void Dispose()
        {
        }

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception, string> formatter)
        {
            output(formatter(state, exception ?? new Exception()));
        }

        bool ILogger.IsEnabled(LogLevel logLevel) => true;

        IDisposable ILogger.BeginScope<TState>(TState state) => this;
    }
}
