using HAFunctions.Shared;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

public class Function
{
    private readonly ILogger<Function> _logger;

    public Function(ILogger<Function> logger)
    {
        _logger = logger;
    }

    [StateTrigger("light.*")]
    public void OnStateChangeLicht(HomeAssistant ha, Event ev) 
    {
      try
      {
        _logger.LogInformation($"State changed of a light: {ev.Data.EntityId}");
      }
      catch(Exception ex)
      {
      	_logger.LogError($"Exception {ex}");        
      }
    }

    [StateTrigger("sensor.*")]
    public void OnStateChangeSensor(HomeAssistant ha, Event ev) 
    {
      try
      {
        _logger.LogInformation($"State changed of a sensor: {ev.Data.EntityId}");
      }
      catch(Exception ex)
      {
      	_logger.LogError($"Exception {ex}");        
      }
      
      if(new Random().NextDouble() > 0.75)
      {
       	throw new Exception("OOF"); 
      }
    }
}