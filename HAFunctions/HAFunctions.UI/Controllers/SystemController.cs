using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HAFunctions.UI.Models;
using HAFunctions.UI.Logging;

namespace HAFunctions.UI.Controllers;

public class SystemController : Controller
{
    private readonly ILogger<SystemController> _logger;

    public SystemController(ILogger<SystemController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Logs()
    {
        return View(InMemoryLogger.LogEntries);
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
