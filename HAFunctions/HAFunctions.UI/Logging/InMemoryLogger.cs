namespace HAFunctions.UI.Logging;

using Microsoft.Extensions.Logging;


public sealed class TypedInMemoryLogger<T> : ILogger<T>
{
    public InMemoryLogger _logger;

    public TypedInMemoryLogger(InMemoryLogger logger)
    {
        _logger = logger;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return _logger.BeginScope<TState>(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _logger.IsEnabled(logLevel);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _logger.Log(logLevel, eventId, state, exception, formatter);
    }
}
public sealed class InMemoryLogger : ILogger
{
    public static InMemoryLogEntry[] LogEntries => _fallbackStore.LogEntries;
    public static InMemoryLogStore _fallbackStore = new InMemoryLogStore();
    private readonly InMemoryLogStore _store;
    private readonly string _name;
    private readonly Func<InMemoryLoggerConfiguration> _getCurrentConfig;

    public InMemoryLogger(
        string name,
        Func<InMemoryLoggerConfiguration> getCurrentConfig)
    {
        _name = name;
        _getCurrentConfig = getCurrentConfig;
        _store = getCurrentConfig().Store ?? _fallbackStore;
    }

    /// <inheritdoc />
    public IDisposable BeginScope<TState>(TState state) where TState : notnull => default!;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }
        _store.AddEntry(new InMemoryLogEntry(DateTime.Now, _name, logLevel.ToString(), formatter(state, exception)), _getCurrentConfig().FunctionFile);
    }
}
