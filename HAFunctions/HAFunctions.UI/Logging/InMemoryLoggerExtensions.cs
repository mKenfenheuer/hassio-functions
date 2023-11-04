using HAFunctions.UI.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

public static class InMemoryLoggerExtensions
{
    public static ILoggingBuilder AddInMemoryLogger(
        this ILoggingBuilder builder)
    {
        builder.AddConfiguration();

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, InMemoryLoggerProvider>());

        LoggerProviderOptions.RegisterProviderOptions
            <InMemoryLoggerConfiguration, InMemoryLoggerProvider>(builder.Services);

        return builder;
    }

    public static ILoggingBuilder AddInMemoryLogger(
        this ILoggingBuilder builder,
        Action<InMemoryLoggerConfiguration> configure)
    {
        builder.AddInMemoryLogger();
        builder.Services.Configure(configure);

        return builder;
    }
}