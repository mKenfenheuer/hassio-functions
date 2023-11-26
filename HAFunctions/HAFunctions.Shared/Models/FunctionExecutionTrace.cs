using System.Text.Json.Serialization;

namespace HAFunctions.Shared.Models;

public class FunctionExecutionTrace
{
    [JsonPropertyName("RunDuration")]
    public double RunDuration { get; set; }
    [JsonPropertyName("Success")]
    public bool Success { get; set; }
    [JsonIgnore]
    public Exception Exception { get; set; }
    [JsonPropertyName("FunctionFile")]
    public string FunctionFile { get; set; }
    [JsonPropertyName("MethodName")]
    public string MethodName { get; set; }
}