using TodoApi.Models;

namespace TodoApi.Services;

public interface IExternalStatusService
{
    Task<IEnumerable<ExternalStatusResult>> CheckStatusAsync(ExternalStatusRequest request, CancellationToken ct);
}
