using System.Reflection;
using System.Text.Json.Serialization;

namespace HAFunctions.Shared.Models;

public class FunctionModel
{
    [JsonPropertyName("FileHash")]
    public string? FileHash { get; set; }
    [JsonPropertyName("FileName")]
    public string? FileName { get; set; }
    [JsonPropertyName("FilePath")]
    public string? FilePath { get; set; }
    [JsonPropertyName("Code")]
    public string? Code { get; set; }
    [JsonPropertyName("DefinedFunctionModels")]

    public MethodModel[]? DefinedFunctionModels { get; set; }

    [JsonIgnore]
    public MethodInfo[]? DefinedFunctions { get; set; }
}