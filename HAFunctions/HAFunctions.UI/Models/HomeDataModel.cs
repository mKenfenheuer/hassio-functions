using HAFunctions.Shared;

namespace HAFunctions.UI.Models;

public class HomeDataModel
{
    public HomeDataModel()
    {
    }

    public Dictionary<string, FunctionExecutionTraceSummary> SummaryByFunctionFile { get; set; }
    public Dictionary<string, FunctionExecutionTraceSummary> SummaryByFunctionFileAndMethodName { get; set; }
    public Dictionary<string, Dictionary<string, FunctionExecutionTraceSummary>> SummaryByFunctionFileSeparateMethodName { get; set; }
    public double AverageRunDuration { get; set; }
    public double TotalSuccessfulExecutions { get; set; }
    public double TotalFailedExecutions { get; set; }
    public double TotalExecutions { get; set; }
    public double FailureRate => TotalFailedExecutions * 100.0 / TotalExecutions;
}