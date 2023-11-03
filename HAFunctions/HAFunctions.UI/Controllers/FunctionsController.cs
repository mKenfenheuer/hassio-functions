using Microsoft.AspNetCore.Mvc;
using HAFunctions.UI.Models;
using HAFunctions.UI.Services;

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
    public IActionResult Edit(string file)
    {
        return View(_store.Functions.FirstOrDefault(f => f.FileName == file));
    }

    [HttpPost]
    public IActionResult Edit([Bind("FileName,Code")] FunctionModel model)
    {
        var fileModel = _store.Functions.FirstOrDefault(f => f.FileName == model.FileName);

        if(fileModel == null)
            return NotFound();
        
        System.IO.File.WriteAllText(fileModel.FilePath,model.Code);

        _store.LoadFunctions();

        return RedirectToAction(nameof(Edit),new {file = fileModel.FileName});
    }

    [HttpPost]
    public IActionResult TryCompile([FromBody] FunctionModel model)
    {
        var result = _compiler.CompileFunctionCode(model.Code, null, false);
        return new JsonResult(result);
    }
}
