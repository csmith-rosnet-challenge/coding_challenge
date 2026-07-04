using System.Diagnostics;
using TodoApi.Models;

namespace TodoApi.Services;

public class ExternalStatusService : IExternalStatusService
{
    private readonly IHttpClientFactory _httpFactory;

    public ExternalStatusService(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    public async Task<IEnumerable<ExternalStatusResult>> CheckStatusAsync(ExternalStatusRequest request, CancellationToken ct)
    {
        var urls = request.Urls.Distinct().ToList();
        var sem = new SemaphoreSlim(Math.Max(1, request.MaxConcurrency));
        var client = _httpFactory.CreateClient();
        client.Timeout = TimeSpan.FromMilliseconds(Math.Max(100, request.TimeoutMs));

        var tasks = new List<Task<ExternalStatusResult>>();

        foreach (var url in urls)
        {
            tasks.Add(Task.Run(async () =>
            {
                await sem.WaitAsync(ct);
                try
                {
                    var sw = Stopwatch.StartNew();
                    try
                    {
                        using var req = new HttpRequestMessage(HttpMethod.Get, url);
                        using var resp = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
                        sw.Stop();
                        return new ExternalStatusResult
                        {
                            Url = url,
                            StatusCode = (int)resp.StatusCode,
                            DurationMs = sw.ElapsedMilliseconds
                        };
                    }
                    catch (TaskCanceledException tex)
                    {
                        sw.Stop();
                        var error = tex.CancellationToken == ct ? "cancelled" : "timeout";
                        return new ExternalStatusResult { Url = url, Error = error, DurationMs = sw.ElapsedMilliseconds };
                    }
                    catch (Exception ex)
                    {
                        sw.Stop();
                        return new ExternalStatusResult { Url = url, Error = ex.Message, DurationMs = sw.ElapsedMilliseconds };
                    }
                }
                finally
                {
                    sem.Release();
                }
            }, ct));
        }

        var results = await Task.WhenAll(tasks);
        return results;
    }
}
