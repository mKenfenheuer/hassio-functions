namespace HAFunctions.Shared;

using System.Text.Json.Serialization;

public class EventMessage : ApiMessage
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("event")]
    public Event Event { get; set; }
}

