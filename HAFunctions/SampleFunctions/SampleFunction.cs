using HAFunctions.Shared;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace HAFunctions.Example;

public class MySampleFunctionClass
{
    private readonly ILogger<MySampleFunctionClass> _logger;

    public MySampleFunctionClass(ILogger<MySampleFunctionClass> logger)
    {
        _logger = logger;
    }

    [StateTrigger("light.licht_kuche", to: "on")]
    public async Task OnStateChangeLichtKücheOn(HomeAssistant ha, Event ev) 
    {
      try
      {
        _logger.LogInformation($"State changed to on: {ev.Data.EntityId}");
      	var result = await ha.Service.Light.Turn_On.Call(target: new {entity_id = "light.licht_flur"});
      }
      catch(Exception ex)
      {
      	_logger.LogError($"Exception {ex}");        
      }
    }

    [StateTrigger("light.licht_kuche", to: "off")]
    public async Task OnStateChangeLichtKücheOff(HomeAssistant ha, Event ev) 
    {
      try
      {
        _logger.LogInformation($"State changed to off: {ev.Data.EntityId}");
      	var result = await ha.Service.Light.Turn_Off.Call(target: new {entity_id = "light.licht_flur"});
      }
      catch(Exception ex)
      {
      	_logger.LogError($"Exception {ex}");        
      }
    }
}