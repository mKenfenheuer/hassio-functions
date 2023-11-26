using Microsoft.AspNetCore.Mvc;
using HAFunctions.UI.Models;
using HAFunctions.UI.Services;
using HAFunctions.Shared.Logging;
using HAFunctions.Shared.Models;
using HAFunctions.Shared.Services;

namespace HAFunctions.UI.Controllers;

public class FunctionsController : Controller
{
    private readonly ILogger<FunctionsController> _logger;
    private readonly FunctionHostService _functionHost;
    private readonly FunctionCompiler _compiler;
    public FunctionsController(ILogger<FunctionsController> logger, FunctionCompiler compiler, FunctionHostService functionHost)
    {
        _logger = logger;
        _compiler = compiler;
        _functionHost = functionHost;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await _functionHost.GetFunctionsAsync());
    }

    [HttpGet]
    public IActionResult New()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> New([Bind("FileName,Code")] FunctionModel model)
    {
        var FunctionCode = System.IO.File.ReadAllText("wwwroot/StartingFunction.cs.txt");
        model.Code = FunctionCode;

        await _functionHost.AddFunction(model);

        return RedirectToAction("Edit", new { file = model.FileName }); ;
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string file)
    {
        var functions = await _functionHost.GetFunctionsAsync();
        return View(functions.FirstOrDefault(f => f.FileName == file));
    }

    [HttpGet]
    public async Task<IActionResult> Log(string file)
    {
        var functions = await _functionHost.GetFunctionsAsync();
        var model = functions.FirstOrDefault(f => f.FileName == file);
        if (model != null)
        {
            var logs = await _functionHost.GetLogsAsync(file);
            return View("/Views/System/Logs.cshtml", logs);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpPost()]
    public async Task<IActionResult> Edit([Bind("FileName,Code")] FunctionModel model)
    {
        var functions = await _functionHost.GetFunctionsAsync();
        var fileModel = functions.FirstOrDefault(f => f.FileName == model.FileName);

        if (fileModel == null)
            return NotFound();

        await _functionHost.UpdateFunction(model);

        return RedirectToAction(nameof(Edit), new { file = fileModel.FileName });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string file)
    {
        var functions = await _functionHost.GetFunctionsAsync();
        var fileModel = functions.FirstOrDefault(f => f.FileName == file);
        return View(fileModel);
    }

    [HttpPost]
    public async Task<IActionResult> Delete([Bind("FileName,Code")] FunctionModel model)
    {
        var functions = await _functionHost.GetFunctionsAsync();
        var fileModel = functions.FirstOrDefault(f => f.FileName == model.FileName);

        if (fileModel == null)
            return NotFound();

        await _functionHost.DeleteFunction(model);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult TryCompile([FromBody] FunctionModel model)
    {
        var result = _compiler.CompileFunctionCode(model.Code, null, false);
        return new JsonResult(result);
    }
}
