using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace HAFunctions.Shared;

public class SubscribeTriggerMessage : ApiCommandMessage
{
    public SubscribeTriggerMessage(dynamic triggerData)
    {
        Type = "subscribe_trigger";
        TriggerData = triggerData;
    }
    
    [JsonPropertyName("trigger")]
    public dynamic TriggerData { get; set; }
}