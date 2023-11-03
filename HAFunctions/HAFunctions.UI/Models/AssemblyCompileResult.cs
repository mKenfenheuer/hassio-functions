using System.Reflection;
using HAFunctions.UI.Services;
using Microsoft.CodeAnalysis;

namespace HAFunctions.UI.Models;

public class AssemblyCompileResult
{
    public Assembly? Assembly { get; set; }
    public bool Success { get; set; }
    public FunctionCompilerDiagnostic[] Diagnostics { get; set; } = new FunctionCompilerDiagnostic[0];
}