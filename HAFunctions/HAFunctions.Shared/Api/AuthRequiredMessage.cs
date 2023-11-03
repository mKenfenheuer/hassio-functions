using System.Text.Json.Serialization;

namespace HAFunctions.Shared;

public class AuthRequiredMessage : ApiMessage
{
    [JsonPropertyName("ha_version")]
    public string HAVersion { get; set; }
}