using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis;

namespace HAFunctions.Shared.Models;

public class FunctionCompilerDiagnostic
{
    [JsonPropertyName("severity")]
    public string Severity { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; }
    [JsonPropertyName("from")]
    public FileLinePosition From { get; set; }
    [JsonPropertyName("to")]
    public FileLinePosition To { get; set; }

    public override string ToString()
    {
        return $"{From} - {To} {Severity}: {Message}";
    }
}

public class FileLinePosition
{
    [JsonPropertyName("line")]
    public int Line { get; set; }
    [JsonPropertyName("ch")]
    public int Character { get; set; }

    public override string ToString()
    {
        return $"L{Line},{Character}";
    }
}