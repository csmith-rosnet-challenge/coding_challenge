using System.Collections.Generic;

namespace TodoApi.Models;

public class ExternalStatusRequest
{
    public List<string> Urls { get; set; } = new List<string>();

    // Timeout per request in milliseconds (optional)
    public int TimeoutMs { get; set; } = 5000;

    // Maximum concurrent requests (optional)
    public int MaxConcurrency { get; set; } = 5;
}
