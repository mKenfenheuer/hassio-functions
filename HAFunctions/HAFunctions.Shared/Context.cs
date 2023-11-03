using Microsoft.Extensions.Logging;

namespace HAFunctions.Shared;
public class Context
{
    public Event Event { get; internal set; }
    public ApiClient ApiClient { get; internal set; }
}
