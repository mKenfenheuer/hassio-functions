using System.Text.Json.Serialization;
using HAFunctions.Shared;

namespace HAFunctions.Shared.Models;

public class FunctionTracesModel
{
    public FunctionTracesModel()
    {
    }

    [JsonPropertyName("SummaryByFunctionFile")]
    public Dictionary<string, FunctionExecutionTraceSummary> SummaryByFunctionFile { get; set; }
    [JsonPropertyName("SummaryByFunctionFileAndMethodName")]
    public Dictionary<string, FunctionExecutionTraceSummary> SummaryByFunctionFileAndMethodName { get; set; }
    [JsonPropertyName("SummaryByFunctionFileSeparateMethodName")]
    public Dictionary<string, Dictionary<string, FunctionExecutionTraceSummary>> SummaryByFunctionFileSeparateMethodName { get; set; }
    [JsonPropertyName("AverageRunDuration")]
    public double AverageRunDuration { get; set; }
    [JsonPropertyName("TotalSuccessfulExecutions")]
    public double TotalSuccessfulExecutions { get; set; }
    [JsonPropertyName("TotalFailedExecutions")]
    public double TotalFailedExecutions { get; set; }
    [JsonPropertyName("TotalExecutions")]
    public double TotalExecutions { get; set; }
    [JsonPropertyName("FailureRate")]
    public double FailureRate => TotalExecutions > 0 ? TotalFailedExecutions * 100.0 / TotalExecutions : 0;
}