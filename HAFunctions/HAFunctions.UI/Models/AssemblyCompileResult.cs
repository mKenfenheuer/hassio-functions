using System.Reflection;
using Microsoft.CodeAnalysis;

namespace HAFunctions.UI.Models;

public class AssemblyCompileResult
{
    public Assembly? Assembly { get; set; }
    public bool Success { get; set; }
    public Diagnostic[] Diagnostics { get; set; } = new Diagnostic[0];
}