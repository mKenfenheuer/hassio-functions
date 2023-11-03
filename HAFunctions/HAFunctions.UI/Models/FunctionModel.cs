using System.Reflection;

namespace HAFunctions.UI.Models;

public class FunctionModel
{
    public string FileName { get; set; }
    public string Code
    {
        get => File.ReadAllText(FileName);
        set => File.WriteAllText(FileName, value);
    }
    public MethodInfo[] DefinedFunctions { get; set; }
}