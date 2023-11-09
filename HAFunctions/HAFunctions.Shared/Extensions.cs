using System.Diagnostics.Metrics;

namespace HAFunctions.Shared;

public static class Extensions
{
    public static async Task<ApiResultMessage> Call(this object obj, dynamic data = null, dynamic target = null)
    {
        if(obj is HomeAssistantService service)
        {
            return await service.Call(data, target);
        }   
        return null;
    }
}