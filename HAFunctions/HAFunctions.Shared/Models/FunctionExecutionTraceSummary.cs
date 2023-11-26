using System.Text.Json.Serialization;

namespace HAFunctions.Shared.Models;

public class FunctionExecutionTraceSummary
{
    public FunctionExecutionTraceSummary()
    {
    }

    public FunctionExecutionTraceSummary(IEnumerable<FunctionExecutionTrace> traces)
    {
        AverageRunDuration = traces.Average(t => t.RunDuration);
        TotalExecutions = traces.Count();
        TotalFailedExecutions = traces.Count(t => !t.Success);
        TotalSuccessfulExecutions = traces.Count(t => t.Success);
        FailedExecutionsByException = traces.Where(t => !t.Success).GroupBy(t => t.Exception).ToDictionary(k => k.Key, v => v.Count());
    }
    [JsonPropertyName("AverageRunDuration")]

    public double AverageRunDuration { get; set; }
    [JsonPropertyName("TotalExecutions")]
    public double TotalExecutions { get; set; }
    [JsonPropertyName("TotalFailedExecutions")]
    public double TotalFailedExecutions { get; set; }
    [JsonPropertyName("TotalSuccessfulExecutions")]
    public double TotalSuccessfulExecutions { get; set; }
    [JsonPropertyName("FailedExecutionsByException")]
    public Dictionary<Exception, int> FailedExecutionsByException { get; set; }
    [JsonPropertyName("FailureRate")]
    public double FailureRate => TotalFailedExecutions * 100.0 / TotalExecutions;
}
