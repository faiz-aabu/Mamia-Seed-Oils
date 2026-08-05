using MamiaSeedsOil.Web.DTOs.Partnership;

namespace MamiaSeedsOil.Web.Interfaces;

public interface IPartnershipApplicationService
{
    Task<PartnershipApplicationResponseDto> SubmitApplicationAsync(PartnershipApplicationRequestDto request, CancellationToken cancellationToken = default);
}
