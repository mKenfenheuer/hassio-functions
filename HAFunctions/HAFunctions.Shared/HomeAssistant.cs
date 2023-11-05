using System.Text.Json;
using System.Text.Json.Nodes;

namespace HAFunctions.Shared;

public class HomeAssistant
{
    private readonly ApiClient ApiClient;

    public HomeAssistant(ApiClient apiClient)
    {
        ApiClient = apiClient;
    }

    public HomeAssistantServiceDomainFactory Service => new HomeAssistantServiceDomainFactory(ApiClient);
}

public class HomeAssistantServiceDomainFactory
{
    private ApiClient ApiClient;

    public HomeAssistantServiceDomainFactory(ApiClient apiClient)
    {
        ApiClient = apiClient;
    }

    public HomeAssistantServiceDomain this[string domain] => new HomeAssistantServiceDomain(ApiClient, domain);
}

public class HomeAssistantServiceDomain
{
    private ApiClient apiClient;
    private string domain;

    public HomeAssistantServiceDomain(ApiClient apiClient, string domain)
    {
        this.apiClient = apiClient;
        this.domain = domain;
    }

    public Func<JsonObject, CallServiceTarget, Task<ApiResultMessage>> this[string service] => new HomeAssistantService(apiClient, domain, service).Call;
}

public class HomeAssistantService
{
    private ApiClient ApiClient;
    public string Domain { get; }
    public string Service { get; }

    public HomeAssistantService(ApiClient apiClient, string domain, string service)
    {
        ApiClient = apiClient;
        Domain = domain;
        Service = service;
    }

    public async Task<ApiResultMessage> Call(JsonObject dynamic, CallServiceTarget target = null) =>  (ApiResultMessage)await ApiClient.SendMessageAsync(new CallServiceApiMessage {
        Domain = Domain,
        Service = Service,
        ServiceData = dynamic,
        Target = target
    });
}