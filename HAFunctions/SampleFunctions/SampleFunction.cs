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

    [NumericStateTrigger("^sensor.*_battery$", below: 10)]
    public async Task OnBatteryStateChangeToProblem(HomeAssistant ha, Event ev) 
    {
      	await ha.Service.Notify.Notify.Call(data: new { message = $"{ev.Data.EntityId.GetEntityIdWithoutDomain().ToPascalCase()} benötigt einen Batteriewechsel!"});
      	_logger.LogInformation("{ev.Data.EntityId.GetEntityIdWithoutDomain().ToPascalCase()} benötigt einen Batteriewechsel!");
    }
}