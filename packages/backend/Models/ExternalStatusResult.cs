namespace Backend.Models;

public class ExternalStatusResult
{
    public string Url { get; set; } = string.Empty;
    public int? StatusCode { get; set; }
    public long DurationMs { get; set; }
    public string? Error { get; set; }
}
