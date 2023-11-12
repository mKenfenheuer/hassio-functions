using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace HAFunctions.Shared;

public class ApiResultMessage : ApiCommandMessage
{
    public ApiResultMessage()
    {
        Type = "result";
    }
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    [JsonPropertyName("error")]
    public ApiError Error { get; set; }
    [JsonPropertyName("result")]
    public dynamic Result { get; set; }
    public ApiResultMessageTyped<T> GetTyped<T>()
    {
        string json = JsonSerializer.Serialize(this);
        return JsonSerializer.Deserialize<ApiResultMessageTyped<T>>(json);
    }
}
