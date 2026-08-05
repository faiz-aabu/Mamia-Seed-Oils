using MamiaSeedsOil.Web.ViewModels;

namespace MamiaSeedsOil.Web.Interfaces;

public interface ISeoService
{
    void ApplySeoMetadata(SeoViewModel seo, IDictionary<string, object?> viewData);
    string BuildOrganizationJsonLd(HomePageViewModel model);
    IReadOnlyList<string> BuildStructuredDataJsonLd(HomePageViewModel model);
    string BuildSitemapXml(string baseUrl, HomePageViewModel model);
    string BuildRobotsTxt(string sitemapUrl);
}
