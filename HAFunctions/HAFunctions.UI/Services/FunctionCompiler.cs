using System.Reflection;
using System.Runtime.Loader;
using HAFunctions.UI.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace HAFunctions.UI.Services;

public class FunctionCompiler 
{
    public AssemblyCompileResult CompileFunctionCode(string code, AssemblyLoadContext loadContext, bool load = true)
    {
        // define source code, then parse it (to the type used for compilation)
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

        // define other necessary objects for compilation
        string assemblyName = Path.GetRandomFileName();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

        var references = assemblies.Where(a => a.Location != null && a.Location != string.Empty).Select(a => MetadataReference.CreateFromFile(a.Location)).ToList();
        references.Add(MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location));
        // analyse and generate IL code from syntax tree
        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName,
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using (var ms = new MemoryStream())
        {
            // write IL code into memory
            EmitResult result = compilation.Emit(ms);

            if (!result.Success)
            {
                // handle exceptions
                IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error);

                return new AssemblyCompileResult()
                {
                    Success = false,
                    Assembly = null,
                        Diagnostics = result.Diagnostics.Select(d => new FunctionCompilerDiagnostic() {
                            Severity = d.Severity.ToString().ToLower(),
                            Message = d.GetMessage(),
                            From = new FileLinePosition() 
                            {
                                Line = d.Location.GetLineSpan().StartLinePosition.Line,
                                Character = d.Location.GetLineSpan().StartLinePosition.Character,
                            },
                            To = new FileLinePosition() 
                            {
                                Line = d.Location.GetLineSpan().EndLinePosition.Line,
                                Character = d.Location.GetLineSpan().EndLinePosition.Character,
                            }
                        }).ToArray(),
                };
            }
            else
            {
                // load this 'virtual' DLL so that we can use
                if(!load)
                    return new AssemblyCompileResult()
                    {
                        Success = true,
                        Assembly = null,
                        Diagnostics = result.Diagnostics.Select(d => new FunctionCompilerDiagnostic() {
                            Severity = d.Severity.ToString().ToLower(),
                            Message = d.GetMessage(),
                            From = new FileLinePosition() 
                            {
                                Line = d.Location.GetLineSpan().StartLinePosition.Line,
                                Character = d.Location.GetLineSpan().StartLinePosition.Character,
                            },
                            To = new FileLinePosition() 
                            {
                                Line = d.Location.GetLineSpan().EndLinePosition.Line,
                                Character = d.Location.GetLineSpan().EndLinePosition.Character,
                            }
                        }).ToArray(),
                    };

                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = loadContext.LoadFromStream(ms);
                return new AssemblyCompileResult()
                {
                    Success = true,
                    Assembly = assembly,
                    Diagnostics = result.Diagnostics.Select(d => new FunctionCompilerDiagnostic() {
                        Severity = d.Severity.ToString().ToLower(),
                        Message = d.GetMessage(),
                        From = new FileLinePosition() 
                        {
                            Line = d.Location.GetLineSpan().StartLinePosition.Line,
                            Character = d.Location.GetLineSpan().StartLinePosition.Character,
                        },
                        To = new FileLinePosition() 
                        {
                            Line = d.Location.GetLineSpan().EndLinePosition.Line,
                            Character = d.Location.GetLineSpan().EndLinePosition.Character,
                        }
                    }).ToArray(),
                };
            }
        }
    }

    public AssemblyCompileResult CompileFunction(string file, AssemblyLoadContext loadContext, bool load = true) => CompileFunctionCode(File.ReadAllText(file), loadContext, load);
}