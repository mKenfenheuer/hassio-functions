using System.Reflection;
using System.Runtime.Loader;

namespace HAFunctions.UI.Models;

public class HAFunctionAssemblyLoadContext : AssemblyLoadContext
{
    private AssemblyDependencyResolver _resolver;
    public bool Unloaded { get; set; }

    public HAFunctionAssemblyLoadContext() : base(isCollectible: true)
    {
        _resolver = new AssemblyDependencyResolver(Path.GetDirectoryName(typeof(object).Assembly.Location));
        Unloading += (s) => Unloaded = true;
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        if (assemblyPath != null)
        {
            return LoadFromAssemblyPath(assemblyPath);
        }
        return null;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        if (libraryPath != null)
        {
            return LoadUnmanagedDllFromPath(libraryPath);
        }
        return IntPtr.Zero;
    }
}