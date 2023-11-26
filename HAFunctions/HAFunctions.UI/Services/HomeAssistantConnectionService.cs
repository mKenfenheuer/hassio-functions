
using HAFunctions.Shared;

namespace HAFunctions.UI.Services;

public class HomeAssistantConnectionService : IHostedService
{
    private readonly ApiClient _api;
    private readonly FunctionStore _functions;
    private readonly ILogger<HomeAssistantConnectionService> _logger;

    public HomeAssistantConnectionService(ApiClient api, FunctionStore functions, ILogger<HomeAssistantConnectionService> logger)
    {
        _api = api;
        _api.OnEventMessageReceived += OnMessageReceived;
        _functions = functions;
        _functions.LoadFunctions();
        _logger = logger;
    }

    private async void OnMessageReceived(object sender, Context context)
    {
        await _functions.CallMatchingFunctions(context);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _api.ConnectAsync(cancellationToken);
        //var result = await _api.SendMessageAsync(new SubscribeEventMessage());
        int id = 0;
        foreach (var function in _functions.Functions)
            foreach (var method in function.DefinedFunctions)
            {
                var attrs = method.GetCustomAttributes(true).Where(a => a is HAFunctionTriggerAttribute).Cast<HAFunctionTriggerAttribute>();
                foreach(var attr in attrs)
                {
                    var data = attr.GetSubscriptionData();
                    var subscribeResult = (ApiResultMessage)await _api.SendMessageAsync(new SubscribeTriggerMessage(data));
                    _functions.SetFunctionTrigger(id.ToString(), method);
                    id++;
                }
            }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _api.DisconnectAsync();
    }
}