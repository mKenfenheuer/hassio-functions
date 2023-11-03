using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HAFunctions.Shared;

public class ApiClient
{
    private readonly ClientWebSocket _ws;
    private readonly Uri _haApiUrl;
    private readonly ILogger<ApiClient> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _accessToken;
    private bool _disconnect = false;
    private Task _receiveTask;
    private CancellationToken _cancellationToken;
    private Dictionary<int, TaskCompletionSource<ApiMessage>> TaskCompletionSources { get; set; } = new Dictionary<int, TaskCompletionSource<ApiMessage>>();
    public event EventHandler<Context> OnEventMessageReceived;
    public bool AutoReconnect { get; set; } = true;
    private int MessageIndex { get; set; } = 1;


    public ApiClient(IConfiguration configuration, ILogger<ApiClient> logger)
    {
        _haApiUrl = new Uri(configuration["HomeAssistant:Uri"] ?? "ws://supervisor/core/websocket");

        if (_haApiUrl.Scheme == "http")
            _haApiUrl = new Uri($"ws://{_haApiUrl.Host}{_haApiUrl.AbsolutePath}");

        if (_haApiUrl.Scheme == "https")
            _haApiUrl = new Uri($"wss://{_haApiUrl.Host}{_haApiUrl.AbsolutePath}");

        if (!_haApiUrl.AbsolutePath.EndsWith("websocket"))
        {
            _haApiUrl = new Uri(new Uri($"{_haApiUrl.Scheme}://{_haApiUrl.Host}{_haApiUrl.AbsolutePath}"), "/api/websocket");
        }

        _accessToken = configuration["HomeAssistant:AccessToken"] ?? Environment.GetEnvironmentVariable("SUPERVISOR_TOKEN");
        _ws = new ClientWebSocket();
        _logger = logger;
    }

    private async Task Receiver()
    {
        try
        {
            while (_ws.State == WebSocketState.Open && !_disconnect)
            {
                try
                {
                    var message = await ReceiveMessageAsync();
                    if (message is ApiResultMessage resultMessage)
                        if (TaskCompletionSources.ContainsKey(resultMessage.Id))
                        {
                            TaskCompletionSources[resultMessage.Id].SetResult(message);
                        }
                    if (message is EventMessage eventMessage)
                    {
                        try
                        {
                            _ = Task.Run(() => OnEventMessageReceived?.Invoke(this, new Context()
                            {
                                Event = eventMessage.Event,
                                ApiClient = this,
                            }));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Failed to receive message from HA Api: {ex}");
                        }
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to receive message from HA Api: {ex}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to continously read messages from HA Api: {ex}");
        }

        if (AutoReconnect && !_cancellationToken.IsCancellationRequested && !_disconnect)
        {
            _logger.LogInformation($"Reconnecting to HomeAssistant WebSocket api at \"{_haApiUrl}\"");
            await ConnectAsync(_cancellationToken);
        }
    }

    private async Task<ApiMessage?> ReceiveMessageAsync()
    {
        ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[10240]);
        var result = await _ws.ReceiveAsync(buffer, _cancellationToken);

        if (!result.EndOfMessage)
        {
            _logger.LogError("Message larger than 10KB!");
            return null;
        }

        string json = Encoding.UTF8.GetString(buffer.Take(result.Count).ToArray());

        var msgType = JsonSerializer.Deserialize<ApiMessage>(json);

        if (msgType?.Type == null)
            return null;


        switch (msgType.Type)
        {
            case "auth_required":
                return JsonSerializer.Deserialize<AuthRequiredMessage>(json);
            case "auth_invalid":
                return JsonSerializer.Deserialize<AuthInvalidMessage>(json);
            case "auth_ok":
                return JsonSerializer.Deserialize<AuthOkMessage>(json);
            case "result":
                return JsonSerializer.Deserialize<ApiResultMessage>(json);
            case "event":
                return JsonSerializer.Deserialize<EventMessage>(json);
        }

        return msgType;
    }

    private async Task SendApiMessageAsync<T>(T message) where T : ApiMessage
    {
        string json = JsonSerializer.Serialize(message);
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        await _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, _cancellationToken);
    }

    public async Task<ApiMessage> SendMessageAsync<T>(T message) where T : ApiCommandMessage
    {
        message.Id = MessageIndex++;
        string json = JsonSerializer.Serialize(message, typeof(T), new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        await _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, _cancellationToken);
        TaskCompletionSource<ApiMessage> taskCompletionSource = new TaskCompletionSource<ApiMessage>();
        TaskCompletionSources[message.Id] = taskCompletionSource;

        var timeout = Task.Run(async () =>
        {
            await Task.Delay(5000);
            if (!taskCompletionSource.Task.IsCompleted)
            {
                taskCompletionSource.SetException(new TimeoutException("Timeout waiting for api response message."));
            }
        });

        return await taskCompletionSource.Task;
    }

    public Task<bool> ConnectAsync() => ConnectAsync(CancellationToken.None);

    public async Task<bool> ConnectAsync(CancellationToken cancellationToken)
    {
        _disconnect = false;
        _cancellationToken = cancellationToken;
        try
        {
            await _ws.ConnectAsync(_haApiUrl, _cancellationToken);

            var authReqMessage = await ReceiveMessageAsync();

            var authMessage = new AuthMessage()
            {
                AccessToken = _accessToken,
            };

            await SendApiMessageAsync(authMessage);

            var msg = await ReceiveMessageAsync();

            _logger.LogInformation($"Now connected to HomeAssistant WebSocket api at \"{_haApiUrl}\"");

            _receiveTask = Receiver();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while connecting to HomeAssistant WebSocket api: {ex}");
            return false;
        }
    }

    public async Task DisconnectAsync()
    {
        _logger.LogInformation($"Disconnecting from HomeAssistant WebSocket api at \"{_haApiUrl}\"");
        _disconnect = true;
        await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Caio.", CancellationToken.None);
    }
}
