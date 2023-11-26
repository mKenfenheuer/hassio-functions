using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HAFunctions.Shared.Logging;
using HAFunctions.UI.Services;

namespace HAFunctions.UI.Controllers;

public class SystemController : Controller
{
    private readonly ILogger<SystemController> _logger;
    private readonly FunctionHostService _functionHost;

    public SystemController(ILogger<SystemController> logger, FunctionHostService functionHost)
    {
        _logger = logger;
        _functionHost = functionHost;
    }

    [HttpGet("Logs")]
    public async Task<InMemoryLogEntry[]> Logs()
    {
        return await _functionHost.GetSystemLogsAsync();
    }
}
