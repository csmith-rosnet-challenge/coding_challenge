using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/external-status/history")]
public class ExternalStatusHistoryController : ControllerBase
{
    private readonly ExternalStatusHistoryService _historyService;

    public ExternalStatusHistoryController(ExternalStatusHistoryService historyService)
    {
        _historyService = historyService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExternalStatusHistory>>> GetLatest([FromQuery] int limit = 100)
    {
        var results = await _historyService.GetLatestAsync(limit);
        return Ok(results);
    }

    [HttpGet("by-url")]
    public async Task<ActionResult<IEnumerable<ExternalStatusHistory>>> GetByUrl([FromQuery] string url, [FromQuery] int limit = 100)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return BadRequest("'url' query parameter is required.");
        }

        var results = await _historyService.GetByUrlAsync(url, limit);
        return Ok(results);
    }
}
