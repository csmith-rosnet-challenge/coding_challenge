using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class ExternalStatusHistoryService
{
    private readonly AppDbContext _db;

    public ExternalStatusHistoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task SaveAsync(IEnumerable<ExternalStatusResult> results)
    {
        var history = results.Select(r => new ExternalStatusHistory
        {
            Url = r.Url,
            StatusCode = r.StatusCode,
            DurationMs = r.DurationMs,
            Error = r.Error,
            CheckedAt = DateTime.UtcNow
        });

        _db.AddRange(history);
        await _db.SaveChangesAsync();
    }

    public async Task<List<ExternalStatusHistory>> GetLatestAsync(int limit = 100)
    {
        return await _db.ExternalStatusHistory
            .OrderByDescending(h => h.CheckedAt)
            .ThenBy(h => h.Url)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<List<ExternalStatusHistory>> GetByUrlAsync(string url, int limit = 100)
    {
        return await _db.ExternalStatusHistory
            .Where(h => h.Url == url)
            .OrderByDescending(h => h.CheckedAt)
            .Take(limit)
            .ToListAsync();
    }
}
