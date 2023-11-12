using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace HAFunctions.Shared;

public class HomeAssistant
{
    private readonly ApiClient ApiClient;
    private readonly static HomeAssistantStateDomainStore stateDomainStore = new HomeAssistantStateDomainStore();

    public HomeAssistant(ApiClient apiClient)
    {
        ApiClient = apiClient;
    }

    public dynamic Service => new HomeAssistantServiceDomainFactory(ApiClient);
    public dynamic States => stateDomainStore;
}

public class HomeAssistantStateDomainStore : DynamicObject
{
    private Dictionary<string, HomeAssistantStateEntityStore> domains = new Dictionary<string, HomeAssistantStateEntityStore>();

    public HomeAssistantStateDomainStore()
    {
    }

    public object this[string property]
    {
        get => GetMember(property);
    }

    public override bool TryGetMember(
        GetMemberBinder binder, out object result)
    {
        result = GetMember(binder.Name);
        return true;
    }

    public object GetMember(string property)
    {
        string domain = property.ToSnakeCase();
        if (!domains.ContainsKey(domain))
        {
            domains[domain] = new HomeAssistantStateEntityStore(domain);
        }

        return domains[domain]; ;
    }
}

public class HomeAssistantStateEntityStore : DynamicObject
{
    private Dictionary<string, HomeAssistantStateEntity> entities = new Dictionary<string, HomeAssistantStateEntity>();

    private string domain;

    public HomeAssistantStateEntityStore(string domain)
    {
        this.domain = domain;
    }

    public object this[string property]
    {
        get => GetMember(property);
    }

    public override bool TryGetMember(
        GetMemberBinder binder, out object result)
    {
        result = GetMember(binder.Name);
        return true;
    }

    public object GetMember(string property)
    {
        string entity = property.ToSnakeCase();
        if (!entities.ContainsKey(entity))
        {
            entities[entity] = new HomeAssistantStateEntity(domain, entity);
        }

        return entities[entity];
    }
}

public class HomeAssistantStateEntity : DynamicObject
{
    private Dictionary<string, object> values = new Dictionary<string, object>();
    private string domain;
    private string entity;

    public HomeAssistantStateEntity(string domain, string entity)
    {
        this.domain = domain;
        this.entity = entity;
    }

    public object this[string property]
    {
        get => GetMember(property);
        set => SetMember(property, value);
    }

    public override bool TryGetMember(
        GetMemberBinder binder, out object result)
    {
        result = GetMember(binder.Name);
        return true;
    }

    public object GetMember(string property)
    {
        string value = property.ToSnakeCase();
        if (!values.ContainsKey(value))
        {
            return "Unknown";
        }
        else
        {
            return values[value];
        }
    }

    public override bool TrySetMember(
        SetMemberBinder binder, object result)
    {
        SetMember(binder.Name, result);
        return true;
    }
    public void SetMember(string property, object value)
    {
        values[property.ToSnakeCase()] = value;
    }
}

public class HomeAssistantServiceDomainFactory : DynamicObject
{
    private ApiClient ApiClient;

    public HomeAssistantServiceDomainFactory(ApiClient apiClient)
    {
        ApiClient = apiClient;
    }

    public override bool TryGetMember(
        GetMemberBinder binder, out object result)
    {
        string domain = binder.Name.ToSnakeCase();
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

    public override bool TryGetMember(
        GetMemberBinder binder, out object result)
    {
        string service = binder.Name.ToSnakeCase();
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