using System.Reflection;
using System.Text.Json.Serialization;

namespace HAFunctions.Shared.Models;

public class MethodModel
{
    [JsonPropertyName("Attributes")]
    public List<AttributeModel> Attributes { get; set; }
    [JsonPropertyName("Parameters")]
    public List<ParameterModel> Parameters { get; set; }
    [JsonPropertyName("Name")]
    public string Name { get; set; }
    [JsonPropertyName("ReturnType")]
    public string ReturnType { get; set; }

    
    public MethodModel(MethodInfo info)
    {
        Attributes = new List<AttributeModel>();
        foreach (var attr in info.CustomAttributes.Where(m => !m.AttributeType.FullName.StartsWith("System")))
        {
            List<AttributeParameterModel> parameters = new List<AttributeParameterModel>();
            for (var i = 0; i < attr.Constructor.GetParameters().Length; i++)
                if (attr.ConstructorArguments[i].Value != null)
                    parameters.Add(new AttributeParameterModel(attr.Constructor.GetParameters()[i], attr.ConstructorArguments[i]));
            Attributes.Add(new AttributeModel(attr.AttributeType.FullName, parameters));
        }

        Parameters = info.GetParameters().Select(m => new ParameterModel(m)).ToList();
        Name = info.Name;
        ReturnType = info.ReturnType.FullName;
    }

    public MethodModel()
    {
    }
}
