﻿using Microsoft.Extensions.Logging;

namespace HAFunctions.Shared;
public class Context
{
    public Event Event { get; set; }
    public ApiClient ApiClient { get; set; }
}
