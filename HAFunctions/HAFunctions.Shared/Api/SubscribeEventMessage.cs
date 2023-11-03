using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace HAFunctions.Shared;

public class SubscribeEventMessage : ApiCommandMessage
{
    public SubscribeEventMessage()
    {
        Type = "subscribe_events";
    }
    
    [JsonPropertyName("event_type")]
    public string EventType { get; set; }
}