using System.Text.Json.Serialization;

namespace HAFunctions.Shared;

public class AuthMessage : ApiMessage
{
    public AuthMessage()
    {
        Type = "auth";
    }

    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
}