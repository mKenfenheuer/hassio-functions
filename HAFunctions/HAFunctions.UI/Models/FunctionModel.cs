using System.Reflection;

namespace HAFunctions.UI.Models;

public class FunctionModel
{
    public string FileHash { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public string Code { get; set; }
    public MethodInfo[] DefinedFunctions { get; set; }
}