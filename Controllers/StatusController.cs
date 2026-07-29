using MamiaSeedsOil.Web.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MamiaSeedsOil.Web.Controllers;

[ApiController]
[Route("status")]
public sealed class StatusController : ControllerBase
{
    private readonly FeatureFlagsOptions _featureFlags;

    public StatusController(IOptions<FeatureFlagsOptions> featureFlags)
    {
        _featureFlags = featureFlags.Value;
    }

    [HttpGet]
    public IActionResult GetStatus()
    {
        if (!_featureFlags.EnableStatusEndpoints)
        {
            return NotFound();
        }

        return Ok(new
        {
            status = "ok",
            utc = DateTimeOffset.UtcNow,
            traceId = HttpContext.TraceIdentifier
        });
    }
}
