using HAFunctions.Shared;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HAFunctions.Example;

public class MySampleFunctionClass
{
    private readonly ILogger<MySampleFunctionClass> _logger;

    public MySampleFunctionClass(ILogger<MySampleFunctionClass> logger)
    {
        _logger = logger;
    }

    [StateTrigger("light.licht_kuche", to: "on")]
    public async Task OnStateChangeLichtKüche(Context context) 
    {
        _logger.LogInformation($"State changed to on: {context.Event.Data.EntityId}");
        await Task.Run(() => {});
    }

    [StateTrigger("sensor.*")]
    public async Task OnStateChange(Context context) 
    {
        _logger.LogInformation($"State changed to {context.Event.Data.NewState.StateValue} - {context.Event.Data.EntityId}");
        await Task.Run(() => {});
    }
}