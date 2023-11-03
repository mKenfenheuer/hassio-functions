
using System.Reflection;
using System.Runtime.Loader;
using HAFunctions.Shared;
using HAFunctions.UI.Models;
using Microsoft.CodeAnalysis;

namespace HAFunctions.UI.Services;

public class FunctionStore
{
    private Dictionary<Type, object> _functionClassInstances = new Dictionary<Type, object>();
    private Dictionary<string, Assembly> _assemblies = new Dictionary<string, Assembly>();
    private readonly FunctionCompiler _compiler;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FunctionStore> _logger;
    private readonly IServiceProvider _serviceProvider;
    private HAFunctionAssemblyLoadContext _assemblyLoadContext;

    public List<FunctionModel> Functions { get; set; } = new List<FunctionModel>();

    public FunctionStore(FunctionCompiler compiler, IConfiguration configuration, ILogger<FunctionStore> logger, IServiceProvider serviceProvider)
    {
        _compiler = compiler;
        _configuration = configuration;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _assemblyLoadContext = new HAFunctionAssemblyLoadContext();
    }

    private void UnloadFunctions()
    {
        _assemblies.Clear();
        Functions.Clear();
        _functionClassInstances.Clear();
        if (!_assemblyLoadContext.Unloaded)
            _assemblyLoadContext.Unload();
        _assemblyLoadContext = new HAFunctionAssemblyLoadContext();
    }

    public void LoadFunctions()
    {
        string directory = _configuration["Storage:FunctionStorageDir"] ?? "/config/HAFunctions/Functions";

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        if (directory == null || directory == string.Empty)
        {
            _logger.LogError("Could not load functions: FunctionStorageDir is empty!");
            return;
        }

        UnloadFunctions();

        foreach (var file in Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories))
        {
            var result = _compiler.CompileFunction(file, _assemblyLoadContext);
            if (result.Success)
            {
                _assemblies.Add(file, result.Assembly);
            }
            else
            {
                string errorSummary = $"Error while compiling HAFunction {Path.GetFileName(file)}:";
                foreach (var diagnostic in result.Diagnostics)
                    errorSummary += $"\r\n    {diagnostic}";
                _logger.LogError(errorSummary);
            }
        }

        foreach (var kvp in _assemblies)
        {
            Functions.Add(new FunctionModel
            {
                FileName = kvp.Key.Replace(directory,""),
                DefinedFunctions = kvp.Value
                                        .GetTypes()
                                        .SelectMany(t => t.GetMethods())
                                        .Where(m => m.GetCustomAttributes(typeof(HAFunctionTriggerAttribute), false).Length > 0)
                                        .ToArray()
            });
        }

        _logger.LogInformation($"Loaded {_assemblies.Count} assemblies with {Functions.Sum(f => f.DefinedFunctions.Length)} methods into FunctionStore.");
    }

    public async Task CallMatchingFunctions(Context context)
    {
        foreach (var method in Functions.SelectMany(f => f.DefinedFunctions))
        {
            if (method.GetCustomAttributes()
            .Where(a => a is HAFunctionTriggerAttribute)
            .Select(a => a as HAFunctionTriggerAttribute)
            .Any(a => a.IsMatch(context.Event)))
            {
                await CallFunction(method, context);
            }
        }
    }

    public async Task CallFunction(MethodInfo info, Context context)
    {
        if (!_functionClassInstances.ContainsKey(info.DeclaringType))
        {
            _functionClassInstances[info.DeclaringType] = CreateInstance(info.DeclaringType, _serviceProvider);
        }
        await Task.Run(() => info.Invoke(_functionClassInstances[info.DeclaringType], new object[] { context }));
    }
    private object CreateInstance(Type type, IServiceProvider provider)
    {
        List<Exception> exceptions = new List<Exception>();
        foreach (var info in type.GetConstructors())
        {
            try
            {
                var obj = CreateInstance(info, provider);
                return obj;
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }
        throw new UnableToCreateInstanceException($"Unable to create instance of type {type}. See inner Exceptions.", exceptions.ToArray());
    }
    private object CreateInstance(ConstructorInfo info, IServiceProvider provider)
    {
        var paramObjs = new List<object>();
        foreach (var param in info.GetParameters())
        {
            var service = provider.GetService(param.ParameterType);
            if (service == null && !param.IsOptional)
            {
                throw new ArgumentException($"Failed to create function instance for {info.DeclaringType}: Could not get service from provider for type {param.ParameterType} and it is not optional.");
            }
            paramObjs.Add(service);
        }
        return info.Invoke(paramObjs.ToArray());
    }
}