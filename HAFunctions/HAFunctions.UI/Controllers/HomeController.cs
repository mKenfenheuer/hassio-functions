using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HAFunctions.UI.Models;
using HAFunctions.Shared;
using HAFunctions.UI.Services;
using HAFunctions.Shared.Models;

namespace HAFunctions.UI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly FunctionHostService _functionHost;

    public HomeController(ILogger<HomeController> logger, FunctionHostService functionHost)
    {
        _logger = logger;
        _functionHost = functionHost;
    }

    public async Task<IActionResult> Index()
    {
        FunctionTracesModel traces = await _functionHost.GetFunctionTracesAsync();
        return View(traces);
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
