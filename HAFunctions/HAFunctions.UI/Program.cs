using System.Reflection;
using HAFunctions.Shared;
using HAFunctions.Shared.Services;
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
        var controllers = builder.Services.AddControllersWithViews();
        
        if (builder.Environment.IsDevelopment())
            controllers.AddRazorRuntimeCompilation();
        
        builder.Services.AddSingleton<FunctionCompiler>();
        builder.Services.AddSingleton<FunctionHostService>();
        builder.Services.AddHostedService<FunctionHostService>();

        app = builder.Build();

        //Enable relative path for HASSIO Ingress
        app.Use(async (context, next) =>
        {
            if (context.Request.Headers.ContainsKey("X-Ingress-Path"))
            {
                context.Request.PathBase = context.Request.Headers["X-Ingress-Path"].ToString();
            }
            await next();
        });

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");

            if (app.Configuration["HSTSEnabled"] == "true")
                app.UseHsts();

            if (app.Configuration["HTTPS:Redirection"] == "true")
                app.UseHttpsRedirection();
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}