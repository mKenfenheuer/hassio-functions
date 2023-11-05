using Microsoft.AspNetCore.Mvc;
using HAFunctions.UI.Models;
using HAFunctions.UI.Services;
using HAFunctions.UI.Logging;

namespace HAFunctions.UI.Controllers;

public class FunctionsController : Controller
{
    private readonly ILogger<FunctionsController> _logger;
    private readonly FunctionStore _store;
    private readonly FunctionCompiler _compiler;
    public FunctionsController(ILogger<FunctionsController> logger, FunctionStore store, FunctionCompiler compiler)
    {
        _logger = logger;
        _store = store;
        _compiler = compiler;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(_store.Functions);
    }

    [HttpGet]
    public IActionResult New()
    {
        return View();
    }


    [HttpPost]
    public IActionResult New([Bind("FileName,Code")] FunctionModel model)
    {
        var FunctionCode = System.IO.File.ReadAllText("wwwroot/StartingFunction.cs.txt");
        model.Code = FunctionCode;

        _store.AddFunction(model);

        return RedirectToAction("Edit", new { file = model.FileName }); ;
    }

    [HttpGet]
    public IActionResult Edit(string file)
    {
        return View(_store.Functions.FirstOrDefault(f => f.FileName == file));
    }

    [HttpGet]
    public IActionResult Log(string file)
    {
        var model = _store.Functions.FirstOrDefault(f => f.FileName == file);

        if (!_store.LogStore.ContainsKey(file))
        {
            if (model != null)
            {
                return View("/Views/System/Logs.cshtml", new InMemoryLogEntry[0]);
            }
            else
            {
                return NotFound();
            }
        }
        return View("/Views/System/Logs.cshtml", _store.LogStore[file].ToArray());
    }

    [HttpPost]
    public IActionResult Edit([Bind("FileName,Code")] FunctionModel model)
    {
        var fileModel = _store.Functions.FirstOrDefault(f => f.FileName == model.FileName);

        if (fileModel == null)
            return NotFound();

        _store.UpdateFunction(model);

        return RedirectToAction(nameof(Edit), new { file = fileModel.FileName });
    }

    [HttpGet]
    public IActionResult Delete(string file)
    {
        return View(_store.Functions.FirstOrDefault(f => f.FileName == file));
    }

    [HttpPost]
    public IActionResult Delete([Bind("FileName,Code")] FunctionModel model)
    {
        var fileModel = _store.Functions.FirstOrDefault(f => f.FileName == model.FileName);

        if (fileModel == null)
            return NotFound();

        _store.DeleteFunction(model);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult TryCompile([FromBody] FunctionModel model)
    {
        var result = _compiler.CompileFunctionCode(model.Code, null, false);
        return new JsonResult(result);
    }
}
