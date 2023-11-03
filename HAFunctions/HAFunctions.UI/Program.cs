using System.Reflection;
using HAFunctions.Shared;
using HAFunctions.UI.Services;

namespace HAFunctions.UI;

public class Program
{
    static Dictionary<Type, object> instances = new Dictionary<Type, object>();
    static WebApplication app;
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddSingleton<FunctionCompiler>();
        builder.Services.AddSingleton<FunctionStore>();
        builder.Services.AddSingleton<ApiClient>();

        builder.Services.AddHostedService<HomeAssistantConnectionService>();

        app = builder.Build();
        
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    } 
}
