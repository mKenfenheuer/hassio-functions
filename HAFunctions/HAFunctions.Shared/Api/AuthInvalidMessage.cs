using System.Text.Json.Serialization;

namespace HAFunctions.Shared;

public class AuthInvalidMessage : ApiMessage
{
    [JsonPropertyName("message")]
    public string Message { get; set; }
}