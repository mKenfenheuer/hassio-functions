using System.Text.Json.Serialization;

namespace HAFunctions.Shared.Logging;

public class InMemoryLogEntry
{
    public InMemoryLogEntry(DateTime time, string source, string severity, string message)
    {
        Time = time;
        Source = source;
        Severity = severity;
        Message = message;
    }
    [JsonPropertyName("Time")]
    public DateTime Time { get; set; }
    [JsonPropertyName("Source")]
    public string Source { get; set; }
    [JsonPropertyName("Severity")]
    public string Severity { get; set; }
    [JsonPropertyName("Message")]
    public string Message { get; set; }
    [JsonPropertyName("BootstrapClass")]
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