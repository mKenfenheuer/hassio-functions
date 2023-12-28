
using System.Reflection;
using System.Security.Cryptography;
using HAFunctions.FunctionsHost.Models;
using HAFunctions.Shared;
using HAFunctions.Shared.Logging;
using HAFunctions.Shared.Models;
using HAFunctions.Shared.Services;

namespace HAFunctions.FunctionsHost.Services;

public class FunctionStore
{
    public InMemoryLogStore LogStore { get; private set; } = new InMemoryLogStore();
    private Dictionary<Type, object> _functionClassInstances = new Dictionary<Type, object>();
    private Dictionary<string, Assembly> _assemblies = new Dictionary<string, Assembly>();
    private readonly FunctionCompiler _compiler;
    private readonly ExecutionTraceStore _traceStore;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FunctionStore> _logger;
    private readonly IServiceProvider _serviceProvider;
    private HAFunctionAssemblyLoadContext _assemblyLoadContext;
    public List<FunctionModel> Functions { get; set; } = new List<FunctionModel>();

    public FunctionStore(FunctionCompiler compiler, IConfiguration configuration, ILogger<FunctionStore> logger, IServiceProvider serviceProvider, ExecutionTraceStore traceStore)
    {
        _compiler = compiler;
        _configuration = configuration;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _assemblyLoadContext = new HAFunctionAssemblyLoadContext();
        _traceStore = traceStore;
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
        string[] dirs = Directory.GetDirectories("/");
        foreach(var dir in dirs)
            _logger.LogInformation(dir);

        string directory = _configuration["Storage:FunctionStorageDir"] ?? "/data/";

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
                _assemblies.Add(file, null);
                string errorSummary = $"Error while compiling HAFunction {Path.GetFileName(file)}:";
                foreach (var diagnostic in result.Diagnostics)
                    errorSummary += $"\r\n    {diagnostic}";
                _logger.LogError(errorSummary);
            }
        }

        foreach (var kvp in _assemblies)
        {
            var defs = kvp.Value?
                                .GetTypes()
                                .SelectMany(t => t.GetMethods())
                                .Where(m => m.GetCustomAttributes(typeof(HAFunctionTriggerAttribute), false).Length > 0)
                                .ToArray() ?? new MethodInfo[0];
            Functions.Add(new FunctionModel
            {
                FileHash = ComputeFileHash(kvp.Key),
                Code = File.ReadAllText(kvp.Key),
                FilePath = kvp.Key,
                FileName = kvp.Key.Replace($"{directory}{Path.DirectorySeparatorChar}", ""),
                DefinedFunctions = defs,
                DefinedFunctionModels = defs.Select(m => new MethodModel(m)).ToArray()
            });
        }

        _logger.LogInformation($"Loaded {_assemblies.Count} assemblies with {Functions.Sum(f => f.DefinedFunctions.Length)} methods into FunctionStore.");
    }

    private string ComputeFileHash(string file)
    {
        SHA1 algorithm = SHA1.Create();
        var hashBytes = algorithm.ComputeHash(File.ReadAllBytes(file));
        return BitConverter.ToString(hashBytes).Replace("-", "");
    }

    public async Task CallMatchingFunctions(Context context)
    {
        List<FunctionModel> models = new List<FunctionModel>();
        lock (Functions)
            models = Functions.ToList();
        foreach (var method in models.SelectMany(f => f.DefinedFunctions))
        {
            if (method.GetCustomAttributes()
            .Where(a => a is HAFunctionTriggerAttribute)
            .Select(a => a as HAFunctionTriggerAttribute)
            .Any(a => a.IsMatch(context.Event.Data.EntityId ?? "", context.Event.Data.OldState, context.Event.Data.NewState)))
            {
                await CallFunction(method, context);
            }
        }
    }

    public async Task CallFunction(MethodInfo info, Context context)
    {
        var model = Functions.FirstOrDefault(f => f.DefinedFunctions.Any(df => df == info));
        var logProvider = new InMemoryLoggerProvider(new InMemoryLoggerConfiguration()
        {
            FunctionFile = model.FileName,
            Store = LogStore
        });
        var logger = logProvider.CreateLogger(info.DeclaringType.FullName);
        var parameters = new List<object>();

        try
        {
            if (!_functionClassInstances.ContainsKey(info.DeclaringType))
            {
                _functionClassInstances[info.DeclaringType] = CreateInstance(info.DeclaringType, _serviceProvider);
            }
            foreach (var paramInfos in info.GetParameters())
            {
                if (paramInfos.ParameterType == typeof(Context))
                {
                    parameters.Add(context);
                }
                if (paramInfos.ParameterType == typeof(Event))
                {
                    parameters.Add(context.Event);
                }
                if (paramInfos.ParameterType == typeof(HomeAssistant))
                {
                    parameters.Add(new HomeAssistant(context.ApiClient));
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Error while constructing arguments for function {model.FileName} - {info.DeclaringType.FullName}.{info.Name}: {ex}");
            _logger.LogError($"Error while constructing arguments for function {model.FileName} - {info.DeclaringType.FullName}.{info.Name}: {ex}");
        }
        DateTime startTime = DateTime.UtcNow;
        try
        {
            var obj = info.Invoke(_functionClassInstances[info.DeclaringType], parameters.ToArray());
            if (obj is Task task)
                await task;
            var duration = DateTime.UtcNow - startTime;
            _traceStore.AddTrace(new FunctionExecutionTrace()
            {
                FunctionFile = model.FileName,
                MethodName = $"{info.DeclaringType.FullName}.{info.Name}",
                Success = true,
                RunDuration = duration.TotalMilliseconds
            });
            logger.LogTrace($"Function execution took {duration.TotalMilliseconds} ms.");
        }
        catch (Exception ex)
        {
            var duration = DateTime.UtcNow - startTime;

            _traceStore.AddTrace(new FunctionExecutionTrace()
            {
                FunctionFile = model.FileName,
                MethodName = $"{info.DeclaringType.FullName}.{info.Name}",
                Success = false,
                RunDuration = duration.TotalMilliseconds,
                Exception = ex
            });

            logger.LogError($"Error while calling function {model.FileName} - {info.DeclaringType.FullName}.{info.Name}: {ex}");
            _logger.LogError($"Error while calling function {model.FileName} - {info.DeclaringType.FullName}.{info.Name}: {ex}");
        }
        GC.Collect();
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
        var model = Functions.FirstOrDefault(f => f.DefinedFunctions.Any(df => df.DeclaringType.GetConstructors().Any(c => c == info)));
        var paramObjs = new List<object>();
        foreach (var param in info.GetParameters())
        {
            if (param.ParameterType == typeof(ILogger) && model != null)
            {
                var logProvider = new InMemoryLoggerProvider(new InMemoryLoggerConfiguration()
                {
                    FunctionFile = model.FileName,
                    Store = LogStore
                });
                paramObjs.Add(logProvider.CreateLogger(info.DeclaringType.FullName));
            }
            else if (param.ParameterType.IsGenericType && param.ParameterType.GetGenericTypeDefinition() == typeof(ILogger<>))
            {
                var logProvider = new InMemoryLoggerProvider(new InMemoryLoggerConfiguration()
                {
                    FunctionFile = model.FileName,
                    Store = LogStore
                });
                Type t = typeof(TypedInMemoryLogger<>);
                Type gt = t.MakeGenericType(param.ParameterType.GenericTypeArguments);
                var obj = gt.GetConstructor(new Type[] { typeof(InMemoryLogger) }).Invoke(new object[] { logProvider.CreateLogger(info.DeclaringType.FullName) });
                paramObjs.Add(obj);
            }
            else
            {
                var service = provider.GetService(param.ParameterType);
                if (service == null && !param.IsOptional)
                {
                    throw new ArgumentException($"Failed to create function instance for {info.DeclaringType}: Could not get service from provider for type {param.ParameterType} and it is not optional.");
                }
                paramObjs.Add(service);
            }
        }
        return info.Invoke(paramObjs.ToArray());
    }

    public void AddFunction(FunctionModel model)
    {
        string directory = _configuration["Storage:FunctionStorageDir"] ?? "/config/HAFunctions/Functions";
        var path = Path.Combine(directory, model.FileName);
        if (File.Exists(path))
        {
            throw new Exception($"File {model.FileName} is already existing. Edit function!");
        }
        File.WriteAllText(path, model.Code);
        LoadFunctions();
    }

    public void UpdateFunction(FunctionModel model)
    {
        string directory = _configuration["Storage:FunctionStorageDir"] ?? "/config/HAFunctions/Functions";
        var path = Path.Combine(directory, model.FileName);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"File {model.FileName} was not found!");
        }
        File.WriteAllText(path, model.Code);
        LoadFunctions();
    }

    public void DeleteFunction(FunctionModel model)
    {
        string directory = _configuration["Storage:FunctionStorageDir"] ?? "/config/HAFunctions/Functions";
        var path = Path.Combine(directory, model.FileName);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"File {model.FileName} was not found!");
        }
        File.Delete(path);
        LoadFunctions();
    }
}