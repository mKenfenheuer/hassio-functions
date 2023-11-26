using System.Text.Json.Serialization;

namespace HAFunctions.Shared.Models;

public class AttributeModel
{
    [JsonPropertyName("Parameters")]
    public List<AttributeParameterModel> Parameters { get; set; }

    [JsonPropertyName("Name")]
    public string Name { get; set; }

    public AttributeModel(string name, List<AttributeParameterModel> parameters)
    {
        Parameters = parameters;
        Name = name;
    }

    public AttributeModel()
    {
    }
}