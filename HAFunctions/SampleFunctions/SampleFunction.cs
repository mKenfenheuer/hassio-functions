using HAFunctions.Shared;
using HAFunctions.Shared.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class BatteryMonitoring
{
  private readonly ILogger<BatteryMonitoring> _logger;

  public BatteryMonitoring(ILogger<BatteryMonitoring> logger)
  {
    _logger = logger;
  }

  [StateTrigger("binary_sensor.fenster_wohnzimmer_mitte_state", to: "on")]
  public async Task OnWohnzimmerWindowOpen(HomeAssistant ha, Event ev, DataStore data)
  {
    await data.Set($"climate_state_wohnzimmer", ha.States.climate.nspanel_thermostat_03_thermostat.temperature);
    await ha.Service.Climate.SetTemperature.Call(new { temperature = 15.0 }, new { entity_id = "climate.nspanel_thermostat_03_thermostat" });
  }
  [StateTrigger("binary_sensor.fenster_wohnzimmer_mitte_state", to: "off")]
  public async Task OnWohnzimmerWindowClose(HomeAssistant ha, Event ev, DataStore data)
  {
    var previous = await data.Get<float>($"climate_state_wohnzimmer");
    var result = await ha.Service.Climate.SetTemperature.Call(new { temperature = previous }, new { entity_id = "climate.nspanel_thermostat_03_thermostat" });
    _logger.LogInformation($"{result}");
  }
}