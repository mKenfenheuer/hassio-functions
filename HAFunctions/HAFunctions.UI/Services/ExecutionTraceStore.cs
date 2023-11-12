namespace HAFunctions.Shared;

public class ExecutionTraceStore
{
    private List<FunctionExecutionTrace> _store = new List<FunctionExecutionTrace>();

    public void AddTrace(FunctionExecutionTrace trace) => _store.Add(trace);

    public FunctionExecutionTrace[] Traces => _store.ToArray();
    public double AverageRunDuration => _store.Average(t => t.RunDuration);
    public double TotalExecutions => _store.Count;
    public double TotalFailedExecutions => _store.Count(t => !t.Success);
    public double TotalSuccessfulExecutions => _store.Count(t => t.Success);
    public Dictionary<Exception, int> FailedExecutionsByException => _store.Where(t => !t.Success).GroupBy(t => t.Exception).ToDictionary(k => k.Key, v => v.Count());
    public Dictionary<string, FunctionExecutionTraceSummary> SummaryByFunctionFileAndMethodName => _store.GroupBy(t => $"{t.FunctionFile} - {t.MethodName}").ToDictionary(k => k.Key, v => new FunctionExecutionTraceSummary(v));
    public Dictionary<string, FunctionExecutionTraceSummary> SummaryByFunctionFile => _store.GroupBy(t => t.FunctionFile).ToDictionary(k => k.Key, v => new FunctionExecutionTraceSummary(v));
    public Dictionary<string, Dictionary<string, FunctionExecutionTraceSummary>> SummaryByFunctionFileSeparateMethodName => _store.GroupBy(t => t.FunctionFile).ToDictionary(k => k.Key, v => v.GroupBy(t => t.MethodName).ToDictionary(key => key.Key, val => new FunctionExecutionTraceSummary(val))); 
}

public class FunctionExecutionTraceSummary
{
    public FunctionExecutionTraceSummary(IEnumerable<FunctionExecutionTrace> traces)
    {
        AverageRunDuration = traces.Average(t => t.RunDuration);
        TotalExecutions = traces.Count();
        TotalFailedExecutions = traces.Count(t => !t.Success);
        TotalSuccessfulExecutions = traces.Count(t => t.Success);
        FailedExecutionsByException = traces.Where(t => !t.Success).GroupBy(t => t.Exception).ToDictionary(k => k.Key, v => v.Count());
    }

    public double AverageRunDuration { get; set; }
    public double TotalExecutions { get; set; }
    public double TotalFailedExecutions { get; set; }
    public double TotalSuccessfulExecutions { get; set; }
    public Dictionary<Exception, int> FailedExecutionsByException { get; set; }
    public double FailureRate => TotalFailedExecutions * 100.0 / TotalExecutions;
}

public class FunctionExecutionTrace
{
    public double RunDuration { get; set; }
    public bool Success { get; set; }
    public Exception Exception { get; set; }
    public string FunctionFile { get; set; }
    public string MethodName { get; set; }
}