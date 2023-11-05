using System.Text.Json.Serialization;

namespace HAFunctions.Shared;
public class CallServiceTarget
{
    [JsonPropertyName("entity_id")]
    public string EntityId { get; set; }
    [JsonPropertyName("area_id")]
    public string AreaId { get; set; }
    [JsonPropertyName("device_id")]
    public string DeviceId { get; set; }
}
