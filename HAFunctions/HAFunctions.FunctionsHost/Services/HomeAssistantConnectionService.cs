
using System.Text.Json;
using HAFunctions.FunctionsHost.Services;

namespace HAFunctions.Shared.Services;

public class HomeAssistantConnectionService : IHostedService
{
    private readonly ApiClient _api;
    private readonly FunctionStore _functions;
    private readonly ILogger<HomeAssistantConnectionService> _logger;

    private bool _connected = false;
    public HomeAssistantConnectionService(ApiClient api, FunctionStore functions, ILogger<HomeAssistantConnectionService> logger)
    {
        _api = api;
        _api.OnMessageReceived += OnMessageReceived;
        _api.OnClientDisconnected += OnClientDisconnected;
        _functions = functions;
        _functions.LoadFunctions();
        _logger = logger;
    }

    private async void OnClientDisconnected(object? sender, EventArgs e)
    {
        if (_connected)
        {
            _connected = false;
            _logger.LogInformation("HA Api client disconnected. Will try to reconnect.");
            await StartAsync(CancellationToken.None);
        }
    }

    private async void OnMessageReceived(object? sender, ApiMessage message)
    {
        try
        {
            if (message is EventMessage stateChanged && stateChanged?.Event?.EventType == "state_changed")
            {
                var ha = new HomeAssistant(_api);
                var newState = stateChanged.Event.Data.NewState;

                var entityState = ha.States[newState.EntityId.GetDomain()][newState.EntityId.GetEntityIdWithoutDomain()];
                entityState.Value = newState.StateValue;
                foreach (var attr in newState.Attributes)
                {
                    entityState[attr.Key] = attr.Value;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while handling state changed message:\nMesssage: {JsonSerializer.Serialize((object)message)}\nException: {ex}");
        }
        try
        {
            if (message is EventMessage eventMessage)
            {
                await _functions.CallMatchingFunctions(new Context()
                {
                    ApiClient = _api,
                    Event = eventMessage.Event
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while handling received message:\nMesssage: {JsonSerializer.Serialize((object)message)}\nException: {ex}");
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var ha = new HomeAssistant(_api);
        var authResult = await _api.ConnectAsync(cancellationToken);
        
        while(!authResult)
        {
            _logger.LogInformation("Connection failed. Retry in 1 sec.");
            await Task.Delay(1000);
            authResult = await _api.ConnectAsync(cancellationToken);
        }

        _connected = true;

        await _api.SendMessageAsync(new SubscribeEventMessage()
        {
            EventType = "state_changed"
        });

        var result = ((ApiResultMessage)await _api.SendMessageAsync(new FetchStatesMessage())).GetTyped<ApiEntityState[]>();
        if (result.Success)
        {
            foreach (var state in result.Result)
            {
                var entityState = ha.States[state.EntityId.GetDomain()][state.EntityId.GetEntityIdWithoutDomain()];
                entityState.Value = state.State;
                foreach (var attr in state.Attributes)
                {
                    entityState[attr.Key] = attr.Value;
                }
            }
        }

        string str = result.ToString();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if(_connected)
        await _api.DisconnectAsync();
        _connected = false;
    }
}