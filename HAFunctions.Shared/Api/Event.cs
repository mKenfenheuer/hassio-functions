namespace HAFunctions.Shared;

using System.Text.Json;


// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
using System.Text.Json.Serialization;

public class Event
{
    [JsonPropertyName("data")]
    public EventData Data { get; set; }

    [JsonPropertyName("event_type")]
    public string EventType { get; set; }

    [JsonPropertyName("time_fired")]
    public DateTime TimeFired { get; set; }

    [JsonPropertyName("origin")]
    public string Origin { get; set; }

    [JsonPropertyName("context")]
    public ApiContext Context { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}

