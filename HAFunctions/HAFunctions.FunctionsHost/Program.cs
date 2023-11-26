
using HAFunctions.FunctionsHost.Services;
using HAFunctions.Shared;
using HAFunctions.Shared.Services;

namespace HAFunctions.FunctionsHost;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<FunctionCompiler>();
        builder.Services.AddSingleton<FunctionStore>();
        builder.Services.AddSingleton<ApiClient>();
        builder.Services.AddSingleton<ExecutionTraceStore>();

        builder.Services.AddLogging(options =>
        {
            options.AddConsole();
            options.AddInMemoryLogger();
        });

        builder.Services.AddHostedService<HomeAssistantConnectionService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();

        app.Run();
    }
}
