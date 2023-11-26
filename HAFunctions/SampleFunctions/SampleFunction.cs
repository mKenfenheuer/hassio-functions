using HAFunctions.Shared;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class BatteryMonitoring
{
  private readonly ILogger<BatteryMonitoring> _logger;

  public BatteryMonitoring(ILogger<BatteryMonitoring> logger)
  {
    _logger = logger;
  }

  [NumericStateTrigger("^.*battery.*$", below: "10")]
  public async Task OnBatteryStateChangeToProblem(HomeAssistant ha, Event ev)
  {
    if (ev.Data.NewState.Attributes["unit_of_measurement"].ToString() == "%" && ev.Data.NewState.Attributes["device_class"].ToString() == "battery")
    {
      await ha.Service.Notify.Notify.Call(data: new { message = $"{ev.Data.EntityId.GetEntityIdWithoutDomain().ToPascalCase()} benötigt einen Batteriewechsel!" });
      _logger.LogInformation("{ev.Data.EntityId.GetEntityIdWithoutDomain().ToPascalCase()} benötigt einen Batteriewechsel ROFL!");
    }
  }

  [StateTrigger(".*")]
  public async Task StateChange(HomeAssistant ha, Event ev)
  {
  }
}