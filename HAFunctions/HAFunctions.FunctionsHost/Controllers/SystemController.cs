using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HAFunctions.Shared.Models;
using HAFunctions.Shared.Logging;

namespace HAFunctions.UI.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class SystemController : ControllerBase
{
    private readonly ILogger<SystemController> _logger;

    public SystemController(ILogger<SystemController> logger)
    {
        _logger = logger;
    }

    [HttpGet("Logs")]
    public InMemoryLogEntry[] Logs()
    {
        return InMemoryLogger.LogEntries;
    }
}
