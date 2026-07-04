namespace Backend.Models;

public class ExternalStatusHistory
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public int? StatusCode { get; set; }
    public long DurationMs { get; set; }
    public string? Error { get; set; }
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
}
