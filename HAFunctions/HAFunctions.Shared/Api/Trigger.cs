namespace HAFunctions.Shared;



// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
using System.Text.Json.Serialization;

public class Trigger
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("idx")]
    public string Idx { get; set; }

    [JsonPropertyName("alias")]
    public object Alias { get; set; }

    [JsonPropertyName("platform")]
    public string Platform { get; set; }

    [JsonPropertyName("entity_id")]
    public string EntityId { get; set; }

    [JsonPropertyName("from_state")]
    public State FromState { get; set; }

    [JsonPropertyName("to_state")]
    public State ToState { get; set; }

    [JsonPropertyName("for")]
    public object For { get; set; }

    [JsonPropertyName("attribute")]
    public object Attribute { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}

