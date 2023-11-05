using System.Collections.Concurrent;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HAFunctions.UI.Logging;

[UnsupportedOSPlatform("browser")]
[ProviderAlias("InMemory")]
public sealed class InMemoryLoggerProvider : ILoggerProvider
{
    private readonly IDisposable? _onChangeToken;
    private InMemoryLoggerConfiguration _currentConfig;
    private readonly ConcurrentDictionary<string, ILogger> _loggers =
        new(StringComparer.OrdinalIgnoreCase);

    public InMemoryLoggerProvider(
        IOptionsMonitor<InMemoryLoggerConfiguration> config)
    {
        _currentConfig = config.CurrentValue;
        _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
    }
    public InMemoryLoggerProvider(
        InMemoryLoggerConfiguration config)
    {
        _currentConfig = config;
    }

    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, name => new InMemoryLogger(name, GetCurrentConfig));

    private InMemoryLoggerConfiguration GetCurrentConfig() => _currentConfig;

    public void Dispose()
    {
        _loggers.Clear();
        _onChangeToken?.Dispose();
    }
}
