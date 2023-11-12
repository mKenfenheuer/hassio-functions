using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HAFunctions.UI.Models;
using HAFunctions.Shared;

namespace HAFunctions.UI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ExecutionTraceStore _traceStore;

    public HomeController(ILogger<HomeController> logger, ExecutionTraceStore traceStore)
    {
        _logger = logger;
        _traceStore = traceStore;
    }

    public IActionResult Index()
    {
        return View(new HomeDataModel() {
            TotalExecutions = _traceStore.TotalExecutions,
            TotalFailedExecutions = _traceStore.TotalFailedExecutions,
            TotalSuccessfulExecutions = _traceStore.TotalSuccessfulExecutions,
            AverageRunDuration = _traceStore.AverageRunDuration,
            SummaryByFunctionFileSeparateMethodName = _traceStore.SummaryByFunctionFileSeparateMethodName,
            SummaryByFunctionFileAndMethodName = _traceStore.SummaryByFunctionFileAndMethodName,
            SummaryByFunctionFile = _traceStore.SummaryByFunctionFile,
        });
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
