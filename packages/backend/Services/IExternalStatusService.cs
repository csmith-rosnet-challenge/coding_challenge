using Backend.Models;

namespace Backend.Services;

public interface IExternalStatusService
{
    Task<IEnumerable<ExternalStatusResult>> CheckStatusAsync(ExternalStatusRequest request, CancellationToken ct);
}
