using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using HAFunctions.Shared.Logging;
using HAFunctions.Shared.Models;

namespace HAFunctions.UI.Services;

public class FunctionHostService : IHostedService
{
    private bool IsRunning = false;
    private readonly Process _hostProcess;
    private readonly ILogger<FunctionHostService> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly string _hostBaseUrl = "http://127.0.0.1:62847";
    private readonly HttpClient _http;
    public FunctionHostService(ILogger<FunctionHostService> logger, IWebHostEnvironment env)
    {
        var location = Path.Combine(Path.GetDirectoryName(typeof(FunctionHostService).Assembly.Location), "HAFunctions.FunctionsHost.dll");
        _hostProcess = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments = $"{location}",
                UseShellExecute = false,
            }
        };
        _hostProcess.StartInfo.EnvironmentVariables["ASPNETCORE_URLS"] = _hostBaseUrl;
        _hostProcess.Exited += (s, e) => OnExit(s, e);
        _logger = logger;
        _env = env;
        _http = new HttpClient();
        _http.BaseAddress = new Uri(_hostBaseUrl);
    }

    private void OnExit(object s, EventArgs e)
    {
        _logger.LogError("FunctionHostService has exited.");
        IsRunning = false;
        StartService();
    }

    private void StartService()
    {
        if (!IsRunning)
        {
            IsRunning = true;
            _hostProcess.Start();
            _logger.LogInformation("FunctionHostService was started.");
        }
    }

    public async Task<FunctionModel[]> GetFunctionsAsync()
    {
        return await JsonSerializer.DeserializeAsync<FunctionModel[]>(await _http.GetStreamAsync("/api/Functions/Index"));
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_env.IsDevelopment())
            StartService();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _hostProcess.Kill();
    }

    public async Task AddFunction(FunctionModel model)
    {
        var result = await JsonSerializer.DeserializeAsync<ActionResultModel>(await (await _http.PostAsJsonAsync("/api/Functions/Function", model)).Content.ReadAsStreamAsync());
    }

    public async Task<InMemoryLogEntry[]> GetLogsAsync(string file)
    {
        return await JsonSerializer.DeserializeAsync<InMemoryLogEntry[]>(await _http.GetStreamAsync($"/api/System/Logs?file={file}"));
    }

    public async Task<InMemoryLogEntry[]> GetSystemLogsAsync()
    {
        return await JsonSerializer.DeserializeAsync<InMemoryLogEntry[]>(await _http.GetStreamAsync("/api/System/Logs"));
    }

    public async Task UpdateFunction(FunctionModel model)
    {
        var result = await JsonSerializer.DeserializeAsync<ActionResultModel>(await (await _http.PatchAsJsonAsync("/api/Functions/Function", model)).Content.ReadAsStreamAsync());
    }

    public async Task DeleteFunction(FunctionModel model)
    {
        var result = await JsonSerializer.DeserializeAsync<ActionResultModel>(await (await _http.DeleteAsync($"/api/Functions/Function?file={model.FileName}")).Content.ReadAsStreamAsync());
    }

    public async Task<FunctionTracesModel> GetFunctionTracesAsync()
    {
        return await JsonSerializer.DeserializeAsync<FunctionTracesModel>(await _http.GetStreamAsync("/api/Traces/Summary"));
    }
}