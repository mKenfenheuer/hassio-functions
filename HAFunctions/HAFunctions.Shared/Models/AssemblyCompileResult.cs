using System.Reflection;
using HAFunctions.Shared.Services;
using Microsoft.CodeAnalysis;

namespace HAFunctions.Shared.Models;

public class AssemblyCompileResult
{
    public Assembly? Assembly { get; set; }
    public bool Success { get; set; }
    public FunctionCompilerDiagnostic[] Diagnostics { get; set; } = new FunctionCompilerDiagnostic[0];
}