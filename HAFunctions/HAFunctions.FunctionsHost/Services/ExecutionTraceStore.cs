using HAFunctions.Shared.Models;

namespace HAFunctions.FunctionsHost.Services;

public class ExecutionTraceStore
{
    private List<FunctionExecutionTrace> _store = new List<FunctionExecutionTrace>();

    public void AddTrace(FunctionExecutionTrace trace) => _store.Add(trace);

    public FunctionExecutionTrace[] Traces => _store.ToArray();
    public double AverageRunDuration => _store.Count > 0 ? _store.Average(t => t.RunDuration) : 0; 
    public double TotalExecutions => _store.Count;
    public double TotalFailedExecutions => _store.Count(t => !t.Success);
    public double TotalSuccessfulExecutions => _store.Count(t => t.Success);
    public Dictionary<Exception, int> FailedExecutionsByException => _store.Where(t => !t.Success).GroupBy(t => t.Exception).ToDictionary(k => k.Key, v => v.Count());
    public Dictionary<string, FunctionExecutionTraceSummary> SummaryByFunctionFileAndMethodName => _store.GroupBy(t => $"{t.FunctionFile} - {t.MethodName}").ToDictionary(k => k.Key, v => new FunctionExecutionTraceSummary(v));
    public Dictionary<string, FunctionExecutionTraceSummary> SummaryByFunctionFile => _store.GroupBy(t => t.FunctionFile).ToDictionary(k => k.Key, v => new FunctionExecutionTraceSummary(v));
    public Dictionary<string, Dictionary<string, FunctionExecutionTraceSummary>> SummaryByFunctionFileSeparateMethodName => _store.GroupBy(t => t.FunctionFile).ToDictionary(k => k.Key, v => v.GroupBy(t => t.MethodName).ToDictionary(key => key.Key, val => new FunctionExecutionTraceSummary(val))); 
}
