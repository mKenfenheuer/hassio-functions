using System.Reflection;
using System.Text.Json.Serialization;

namespace HAFunctions.Shared.Models;

public class AttributeParameterModel
{
    public AttributeParameterModel()
    {
    }

    public AttributeParameterModel(ParameterInfo parameterInfo, CustomAttributeTypedArgument customAttributeTypedArgument)
    {
        Type = parameterInfo.ParameterType.FullName;
        Name = parameterInfo.Name;
        Value = customAttributeTypedArgument.Value?.ToString() ?? "null";
    }

    [JsonPropertyName("Type")]
    public string Type { get; set; }
    [JsonPropertyName("Name")]
    public string Name { get; set; }
    [JsonPropertyName("Value")]
    public string Value { get; set; }
}
