using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers;

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
    public async Task<ActionResult<IEnumerable<ExternalStatusResult>>> GatherDependencyStatus([FromBody] ExternalStatusRequest request, CancellationToken ct)
    {
        if (request == null || request.Urls == null || request.Urls.Count == 0)
            return BadRequest("Request must include a non-empty 'urls' array.");

        var results = await _statusService.CheckStatusAsync(request, ct);
        return Ok(results);
    }
}
