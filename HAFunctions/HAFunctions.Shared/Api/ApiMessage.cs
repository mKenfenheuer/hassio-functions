using System.Text.Json.Serialization;

namespace HAFunctions.Shared;

public class ApiMessage
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
}

public class ApiCommandMessage : ApiMessage{

    [JsonPropertyName("id")]
    public int Id { get; set; }
}