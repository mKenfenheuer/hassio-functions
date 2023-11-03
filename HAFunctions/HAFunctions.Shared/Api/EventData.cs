namespace HAFunctions.Shared;


// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
using System.Text.Json.Serialization;

public class EventData
{
    [JsonPropertyName("entity_id")]
    public string EntityId { get; set; }

    [JsonPropertyName("new_state")]
    public State NewState { get; set; }

    [JsonPropertyName("old_state")]
    public State OldState { get; set; }
}

