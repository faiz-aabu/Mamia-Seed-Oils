using MamiaSeedsOil.Web.Configuration;
using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Models;
using Microsoft.Extensions.Options;

namespace MamiaSeedsOil.Web.Services;

public sealed class CompanyProfileService : ICompanyProfileService
{
    private readonly CompanyProfileOptions _options;

    public CompanyProfileService(IOptions<CompanyProfileOptions> options)
    {
        _options = options.Value;
    }

    public Task<CompanyProfile> GetCompanyProfileAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new CompanyProfile
        {
            CompanyName = _options.CompanyName,
            EstablishedDate = _options.EstablishedDate,
            AddressLines = _options.AddressLines,
            BusinessDescription = _options.BusinessDescription,
            AdditionalProductsDescription = _options.AdditionalProductsDescription
        });
    }
}
