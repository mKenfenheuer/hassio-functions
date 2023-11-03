using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HAFunctions.UI.Models;
using HAFunctions.UI.Services;

namespace HAFunctions.UI.Controllers;

public class FunctionsController : Controller
{
    private readonly ILogger<FunctionsController> _logger;
    private readonly FunctionStore _store;
    public FunctionsController(ILogger<FunctionsController> logger, FunctionStore store)
    {
        _logger = logger;
        _store = store;
    }

    public IActionResult Index()
    {
        return View(_store.Functions);
    }
}
