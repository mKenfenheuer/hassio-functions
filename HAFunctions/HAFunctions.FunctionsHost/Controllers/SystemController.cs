using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HAFunctions.Shared.Models;
using HAFunctions.Shared.Logging;
using HAFunctions.FunctionsHost.Services;

namespace HAFunctions.UI.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class SystemController : ControllerBase
{
    private readonly ILogger<SystemController> _logger;
    private readonly FunctionStore _store;

    public SystemController(ILogger<SystemController> logger, FunctionStore store)
    {
        _logger = logger;
        _store = store;
    }

    [HttpGet("Logs")]
    public InMemoryLogEntry[] Logs()
    {
        return InMemoryLogger.LogEntries;
    }

    [HttpGet("FunctionLogs")]
    public IEnumerable<InMemoryLogEntry> GetLogs([FromQuery] string file = null)
    {
        if(file == null)
            return _store.LogStore.LogEntries;
            
        if (_store.LogStore.ContainsKey(file))
            return _store.LogStore[file];
        else
            return new InMemoryLogEntry[0];
    }
}
