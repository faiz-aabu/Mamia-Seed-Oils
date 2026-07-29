using MamiaSeedsOil.Web.ViewModels;

namespace MamiaSeedsOil.Web.Interfaces;

public interface ISeoService
{
    void ApplySeoMetadata(SeoViewModel seo, IDictionary<string, object?> viewData);
    string BuildOrganizationJsonLd(HomePageViewModel model);
    IReadOnlyList<string> BuildStructuredDataJsonLd(HomePageViewModel model);
    IReadOnlyList<KeyValuePair<string, string>> BuildHreflangLinks(string baseUrl, string currentPath, IReadOnlyList<string> supportedCultures, string defaultCulture);
    string BuildSitemapXml(string baseUrl, HomePageViewModel model, IReadOnlyList<string> supportedCultures, string defaultCulture);
    string BuildRobotsTxt(string sitemapUrl);
}
