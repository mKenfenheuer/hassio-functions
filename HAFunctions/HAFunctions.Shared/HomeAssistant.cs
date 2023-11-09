using System.Dynamic;
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

    public dynamic Service => new HomeAssistantServiceDomainFactory(ApiClient);
}

public class HomeAssistantServiceDomainFactory : DynamicObject
{
    private ApiClient ApiClient;

    public HomeAssistantServiceDomainFactory(ApiClient apiClient)
    {
        ApiClient = apiClient;
    }
    // If you try to get a value of a property
    // not defined in the class, this method is called.
    public override bool TryGetMember(
        GetMemberBinder binder, out object result)
    {
        // Converting the property name to lowercase
        // so that property names become case-insensitive.
        string domain = binder.Name.ToLower();

        // If the property name is found in a dictionary,
        // set the result parameter to the property value and return true.
        // Otherwise, return false.
        result = new HomeAssistantServiceDomain(ApiClient, domain);
        return true;
    }
}

public class HomeAssistantServiceDomain : DynamicObject
{
    private ApiClient ApiClient;
    private string domain;

    public HomeAssistantServiceDomain(ApiClient apiClient, string domain)
    {
        this.ApiClient = apiClient;
        this.domain = domain;
    }


    // If you try to get a value of a property
    // not defined in the class, this method is called.
    public override bool TryGetMember(
        GetMemberBinder binder, out object result)
    {
        // Converting the property name to lowercase
        // so that property names become case-insensitive.
        string service = binder.Name.ToLower();

        // If the property name is found in a dictionary,
        // set the result parameter to the property value and return true.
        // Otherwise, return false.
        result = new HomeAssistantService(ApiClient, domain, service);
        return true;
    }
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

    public async Task<ApiResultMessage> Call(dynamic data = null, dynamic target = null)
    {
        return (ApiResultMessage)await ApiClient.SendMessageAsync(new CallServiceApiMessage
        {
            Domain = Domain,
            Service = Service,
            ServiceData = data,
            Target = target
        });
    }
}