using System.Text.Json.Serialization;

namespace HAFunctions.Shared;

public class ApiError
{
    [JsonPropertyName("code")]
    public string Code { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; }
}