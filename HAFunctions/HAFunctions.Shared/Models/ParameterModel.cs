using System.Reflection;
using System.Text.Json.Serialization;

namespace HAFunctions.Shared.Models;

public class ParameterModel
{
    [JsonPropertyName("Type")]
    public string Type { get; set; }
    [JsonPropertyName("Name")]
    public string Name { get; set; }
    [JsonPropertyName("DefaultValue")]
    public string DefaultValue { get; set; }
    public ParameterModel(ParameterInfo m)
    {
        Type = m.ParameterType.FullName;
        Name = m.Name;
        DefaultValue = m.DefaultValue?.ToString() ?? "null";
    }

    public ParameterModel()
    {
    }
}
