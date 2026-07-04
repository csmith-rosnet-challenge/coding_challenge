using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExternalStatusController : ControllerBase
{
    private readonly IExternalStatusService _statusService;

    public ExternalStatusController(IExternalStatusService statusService)
    {
        _statusService = statusService;
    }

    [HttpPost]
    public async Task<ActionResult<IEnumerable<ExternalStatusResult>>> GatherDependencyStatus([FromBody] ExternalStatusRequest? request, CancellationToken ct)
    {
        request ??= new ExternalStatusRequest();
        var results = await _statusService.CheckStatusAsync(request, ct);
        return Ok(results);
    }
}
