namespace HAFunctions.UI.Logging;

using Microsoft.Extensions.Logging;

public sealed class InMemoryLogger : ILogger
{
    public static InMemoryLogEntry[] LogEntries => _logEntries.ToArray();
    private static List<InMemoryLogEntry> _logEntries = new List<InMemoryLogEntry>();
    private readonly string _name;
    private readonly Func<InMemoryLoggerConfiguration> _getCurrentConfig;

    public InMemoryLogger(
        string name,
        Func<InMemoryLoggerConfiguration> getCurrentConfig) =>
        (_name, _getCurrentConfig) = (name, getCurrentConfig);

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

        _logEntries.Add(new InMemoryLogEntry(DateTime.Now, _name, logLevel.ToString(),formatter(state, exception)));
    }
}
