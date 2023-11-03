using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace HAFunctions.Shared;

public class ApiResultMessageTyped<T> : ApiCommandMessage
{
    public ApiResultMessageTyped()
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
    public T Result { get; set; }

}
