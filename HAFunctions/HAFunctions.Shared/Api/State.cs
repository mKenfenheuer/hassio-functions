namespace HAFunctions.Shared;

using System.Text.Json.Nodes;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
using System.Text.Json.Serialization;

public class State
{
    [JsonPropertyName("entity_id")]
    public string EntityId { get; set; }

    [JsonPropertyName("last_changed")]
    public DateTime LastChanged { get; set; }

    [JsonPropertyName("state")]
    public string StateValue { get; set; }

    [JsonPropertyName("attributes")]
    public JsonObject Attributes { get; set; }

    [JsonPropertyName("last_updated")]
    public DateTime LastUpdated { get; set; }

    [JsonPropertyName("context")]
    public Context Context { get; set; }
}

