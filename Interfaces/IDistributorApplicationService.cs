using MamiaSeedsOil.Web.DTOs;

namespace MamiaSeedsOil.Web.Interfaces;

public interface IDistributorApplicationService
{
    Task<DistributorApplicationResponseDto> SubmitApplicationAsync(DistributorApplicationRequestDto request, CancellationToken cancellationToken = default);
}
