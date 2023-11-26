
namespace HAFunctions.Shared.Logging;

public class InMemoryLogStore
{
    private List<InMemoryLogEntry> _logEntries = new List<InMemoryLogEntry>();
    private Dictionary<string, List<InMemoryLogEntry>> _logEntriesByFunction = new Dictionary<string, List<InMemoryLogEntry>>();
    public InMemoryLogEntry[] LogEntries => _logEntries.ToArray();
    public InMemoryLogEntry[] this[string file] => _logEntriesByFunction[file].ToArray();
    public void AddEntry(InMemoryLogEntry entry, string file = "default")
    {
        file = file ?? "default";
        if (!_logEntriesByFunction.ContainsKey(file))
            _logEntriesByFunction[file] = new List<InMemoryLogEntry>();
        
        _logEntriesByFunction[file].Add(entry);
        _logEntries.Add(entry);
    }

    public bool ContainsKey(string file) => _logEntriesByFunction.ContainsKey(file);
}