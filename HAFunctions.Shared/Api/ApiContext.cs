using System.Text.Json.Serialization;

namespace HAFunctions.Shared;

public class ApiContextResult
{
    [JsonPropertyName("context")]
    public ApiContext Context { get; set; }

}
public class ApiContext
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("parent_id")]
    public string ParentId { get; set; }
    [JsonPropertyName("user_id")]
    public string UserId { get; set; }
}