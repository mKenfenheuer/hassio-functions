namespace HAFunctions.Shared;



// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
using System.Text.Json.Serialization;

public class Variables
{
    [JsonPropertyName("trigger")]
    public Trigger Trigger { get; set; }
}

