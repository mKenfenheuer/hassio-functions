namespace HAFunctions.UI.Logging;

public class InMemoryLogEntry
{
    public InMemoryLogEntry(DateTime time, string source, string severity, string message)
    {
        Time = time;
        Source = source;
        Severity = severity;
        Message = message;
    }

    public DateTime Time { get; set; }
    public string Source { get; set; }
    public string Severity { get; set; }
    public string Message { get; set; }
    public string BootstrapClass
    {
        get
        {
            switch (Severity)
            {
                case "Debug":
                    return "text-info";
                case "Information":
                    return "text-success";
                case "Warning":
                    return "text-warning";
                case "Error":
                case "Critical":
                    return "text-danger";
                case "Trace":
                default:
                    return "";
            }
        }
    }
}