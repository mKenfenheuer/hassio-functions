using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HAFunctions.Shared.Models;
using HAFunctions.Shared.Logging;
using HAFunctions.FunctionsHost.Services;

namespace HAFunctions.UI.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class TracesController : ControllerBase
{
    private readonly ExecutionTraceStore _traceStore;
    private readonly ILogger<SystemController> _logger;

    public TracesController(ILogger<SystemController> logger, ExecutionTraceStore traceStore)
    {
        _logger = logger;
        _traceStore = traceStore;
    }

    [HttpGet("Summary")]
    public FunctionTracesModel TraceSummary()
    {
        return new FunctionTracesModel() {
            TotalExecutions = _traceStore.TotalExecutions,
            TotalFailedExecutions = _traceStore.TotalFailedExecutions,
            TotalSuccessfulExecutions = _traceStore.TotalSuccessfulExecutions,
            AverageRunDuration = _traceStore.AverageRunDuration,
            SummaryByFunctionFileSeparateMethodName = _traceStore.SummaryByFunctionFileSeparateMethodName,
            SummaryByFunctionFileAndMethodName = _traceStore.SummaryByFunctionFileAndMethodName,
            SummaryByFunctionFile = _traceStore.SummaryByFunctionFile,
        };
    }
}
