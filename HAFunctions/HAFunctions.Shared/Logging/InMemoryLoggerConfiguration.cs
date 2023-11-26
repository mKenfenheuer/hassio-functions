namespace HAFunctions.Shared.Logging;

public class InMemoryLoggerConfiguration
{
    public InMemoryLogStore Store { get; set; }
    public string FunctionFile { get; set; }

    public InMemoryLoggerConfiguration(InMemoryLogStore store = null, string functionFile = null)
    {
        Store = store;
        FunctionFile = functionFile;
    }

    public InMemoryLoggerConfiguration()
    {
    }
}