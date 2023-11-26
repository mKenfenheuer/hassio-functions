using System.Text.Json.Serialization;

namespace HAFunctions.Shared.Models;

public class ActionResultModel
{
    [JsonPropertyName("Message")]
    public string Message { get; set; }
    [JsonPropertyName("Success")]
    public bool Success { get; set; }
}