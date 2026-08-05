using MamiaSeedsOil.Web.Models;

namespace MamiaSeedsOil.Web.Interfaces;

public interface ICompanyProfileService
{
    Task<CompanyProfile> GetCompanyProfileAsync(CancellationToken cancellationToken = default);
}
