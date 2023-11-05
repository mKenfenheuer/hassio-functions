using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace HAFunctions.Shared;

public class CallServiceApiMessage : ApiCommandMessage
{
    public CallServiceApiMessage()
    {
        Type = "call_service";
    }

    [JsonPropertyName("domain")]
    public string Domain { get; set; }
    [JsonPropertyName("service")]
    public string Service { get; set; }

    [JsonPropertyName("service_data")]
    public JsonObject ServiceData { get; set; } = new JsonObject();

    [JsonPropertyName("target")]
    public CallServiceTarget Target { get; set; }
}