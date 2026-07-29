using MamiaSeedsOil.Web.DTOs.Partnership;
using MamiaSeedsOil.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MamiaSeedsOil.Web.Controllers;

[ApiController]
[Route("api/partnership")]
public sealed class PartnershipController : ControllerBase
{
    private readonly IPartnershipApplicationService _partnershipApplicationService;

    public PartnershipController(IPartnershipApplicationService partnershipApplicationService)
    {
        _partnershipApplicationService = partnershipApplicationService;
    }

    [HttpPost("applications")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult<PartnershipApplicationResponseDto>> SubmitApplication([FromBody] PartnershipApplicationRequestDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = await _partnershipApplicationService.SubmitApplicationAsync(request, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
