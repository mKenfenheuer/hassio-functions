using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HAFunctions.Shared.Models;
using HAFunctions.Shared.Logging;
using HAFunctions.FunctionsHost.Services;

namespace HAFunctions.FunctionsHost.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class FunctionsController : ControllerBase
{
    private readonly ILogger<FunctionsController> _logger;
    private readonly FunctionStore _store;

    public FunctionsController(ILogger<FunctionsController> logger, FunctionStore store)
    {
        _logger = logger;
        _store = store;
    }

    [HttpGet("Index")]
    public IEnumerable<FunctionModel> Index()
    {
        return _store.Functions;
    }

    [HttpGet("Function")]
    public IEnumerable<FunctionModel> GetFunction([FromQuery] string file)
    {
        return _store.Functions.Where(f => f.FileName == file);
    }

    [HttpPost("Function")]
    public FunctionModel AddFunction([FromBody] FunctionModel model)
    {
        _store.AddFunction(model);
        return model;
    }

    [HttpPatch("Function")]
    public ActionResultModel UpdateFunction([FromBody] FunctionModel model)
    {
        try
        {
            _store.UpdateFunction(model);

            return new ActionResultModel()
            {
                Success = true,
                Message = "Function has been updated.",
            };
        }
        catch (Exception ex)
        {
            return new ActionResultModel()
            {
                Success = false,
                Message = $"Error while updating function: {ex}",
            };
        }
    }

    [HttpDelete("Function")]
    public ActionResultModel DeleteFunction([FromQuery] string file)
    {
        try
        {
            var model = _store.Functions.FirstOrDefault(f => f.FileName == file);
            _store.DeleteFunction(model);

            return new ActionResultModel()
            {
                Success = true,
                Message = "Function has been deleted.",
            };
        }
        catch (Exception ex)
        {
            return new ActionResultModel()
            {
                Success = false,
                Message = $"Error while deleting function: {ex}",
            };
        }
    }
}
