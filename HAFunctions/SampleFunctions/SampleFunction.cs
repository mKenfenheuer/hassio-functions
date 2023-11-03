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

    [StateTrigger("light.licht_kuche")]
    public async Task OnStateChangeLichtKüche(Context context) 
    {
        _logger.LogInformation($"Received state change on {context.Event.Data.EntityId}");
        await Task.Run(() => {});
    }

    [StateTrigger("light.licht_flur")]
    public async Task OnStateChangeLichtFlur(Context context) 
    {
        _logger.LogInformation($"New State change on {context.Event.Data.EntityId}");
        await Task.Run(() => {});
    }
}