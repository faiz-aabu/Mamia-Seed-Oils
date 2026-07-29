using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.ViewModels;
using System.Globalization;

namespace MamiaSeedsOil.Web.Services;

public sealed class SeoService : ISeoService
{
    public void ApplySeoMetadata(SeoViewModel seo, IDictionary<string, object?> viewData)
    {
        viewData["Title"] = seo.Title;
        viewData["SeoDescription"] = seo.Description;
        viewData["SeoKeywords"] = seo.Keywords;
        viewData["CanonicalUrl"] = seo.CanonicalUrl;
        viewData["OgImageUrl"] = seo.OgImageUrl;
        viewData["TwitterImageUrl"] = seo.TwitterImageUrl;
    }

    public string BuildOrganizationJsonLd(HomePageViewModel model)
    {
        var payload = BuildOrganizationSchema(model);
        return JsonSerializer.Serialize(payload);
    }

    public IReadOnlyList<string> BuildStructuredDataJsonLd(HomePageViewModel model)
    {
        var list = new List<string>
        {
            JsonSerializer.Serialize(BuildOrganizationSchema(model)),
            JsonSerializer.Serialize(BuildLocalBusinessSchema(model)),
            JsonSerializer.Serialize(BuildProductItemListSchema(model)),
            JsonSerializer.Serialize(BuildFaqSchema(model))
        };

        return list;
    }

    public IReadOnlyList<KeyValuePair<string, string>> BuildHreflangLinks(string baseUrl, string currentPath, IReadOnlyList<string> supportedCultures, string defaultCulture)
    {
        var list = new List<KeyValuePair<string, string>>();
        var normalizedPath = NormalizePathWithoutCulturePrefix(currentPath, supportedCultures);

        foreach (var culture in supportedCultures)
        {
            var href = BuildLocalizedUrl(baseUrl, normalizedPath, culture, defaultCulture);
            list.Add(new KeyValuePair<string, string>(culture, href));
        }

        var xDefault = BuildLocalizedUrl(baseUrl, normalizedPath, defaultCulture, defaultCulture);
        list.Add(new KeyValuePair<string, string>("x-default", xDefault));

        return list;
    }

    public string BuildSitemapXml(string baseUrl, HomePageViewModel model, IReadOnlyList<string> supportedCultures, string defaultCulture)
    {
        var now = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var basePaths = new List<string>
        {
            "/"
        };

        basePaths.AddRange(model.NavigationItems
            .Select(item => item.Anchor)
            .Where(anchor => !string.IsNullOrWhiteSpace(anchor))
            .Select(anchor => $"/{anchor.TrimStart('#')}"));

        basePaths.AddRange(model.Products
            .Where(p => !string.IsNullOrWhiteSpace(p.SeoUrl))
            .Select(p => $"/products/{p.SeoUrl}"));

        var urls = new List<string>();
        foreach (var basePath in basePaths.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            foreach (var culture in supportedCultures)
            {
                urls.Add(BuildLocalizedUrl(baseUrl, basePath, culture, defaultCulture));

                if (string.Equals(culture, defaultCulture, StringComparison.OrdinalIgnoreCase))
                {
                    var normalizedPath = string.IsNullOrWhiteSpace(basePath) ? "/" : basePath;
                    if (!normalizedPath.StartsWith('/'))
                    {
                        normalizedPath = "/" + normalizedPath;
                    }

                    urls.Add($"{baseUrl.TrimEnd('/')}/{culture}{normalizedPath}");
                }
            }
        }

        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        var doc = new XDocument(
            new XElement(ns + "urlset",
                urls.Distinct(StringComparer.OrdinalIgnoreCase).Select(url =>
                    new XElement(ns + "url",
                        new XElement(ns + "loc", url),
                        new XElement(ns + "lastmod", now),
                        new XElement(ns + "changefreq", "weekly"),
                        new XElement(ns + "priority", "0.8")))));

        return doc.ToString();
    }

    public string BuildRobotsTxt(string sitemapUrl)
    {
        var content = new StringBuilder();
        content.AppendLine("User-agent: *");
        content.AppendLine("Allow: /");
        content.AppendLine("Disallow: /error/");
        content.AppendLine();
        content.AppendLine($"Sitemap: {sitemapUrl}");
        return content.ToString();
    }

    private static object BuildOrganizationSchema(HomePageViewModel model)
    {
        var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        return new
        {
            @context = "https://schema.org",
            @type = "Organization",
            name = model.Company.Name,
            inLanguage = language,
            url = model.Seo.CanonicalUrl,
            logo = model.Seo.SchemaLogoUrl,
            description = model.Seo.Description,
            address = new
            {
                @type = "PostalAddress",
                streetAddress = model.Company.AddressLine1,
                addressLocality = model.Company.City,
                addressRegion = model.Company.Region,
                addressCountry = "NG"
            },
            contactPoint = new
            {
                @type = "ContactPoint",
                telephone = model.Company.Phone,
                contactType = "sales",
                areaServed = "NG",
                email = model.Company.Email
            },
            sameAs = model.Company.SocialLinks.Values
        };
    }

    private static object BuildLocalBusinessSchema(HomePageViewModel model)
    {
        var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        return new
        {
            @context = "https://schema.org",
            @type = "LocalBusiness",
            name = model.Company.Name,
            inLanguage = language,
            image = model.Seo.OgImageUrl,
            url = model.Seo.CanonicalUrl,
            telephone = model.Company.Phone,
            email = model.Company.Email,
            address = new
            {
                @type = "PostalAddress",
                streetAddress = model.Company.AddressLine1,
                addressLocality = model.Company.City,
                addressRegion = model.Company.Region,
                addressCountry = model.Company.Country
            },
            openingHours = model.Company.BusinessHours,
            areaServed = model.Distribution.Regions.Select(region => region.Title)
        };
    }

    private static object BuildProductItemListSchema(HomePageViewModel model)
    {
        var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        return new
        {
            @context = "https://schema.org",
            @type = "ItemList",
            inLanguage = language,
            itemListElement = model.Products.Select((product, index) => new
            {
                @type = "ListItem",
                position = index + 1,
                item = new
                {
                    @type = "Product",
                    name = product.Name,
                    description = product.Description,
                    sku = product.SeoUrl,
                    category = product.Category,
                    image = product.Image,
                    additionalProperty = new[]
                    {
                        new { @type = "PropertyValue", name = "PackagingSize", value = product.PackagingSize },
                        new { @type = "PropertyValue", name = "Availability", value = product.Availability }
                    }
                }
            })
        };
    }

    private static object BuildFaqSchema(HomePageViewModel model)
    {
        var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        return new
        {
            @context = "https://schema.org",
            @type = "FAQPage",
            inLanguage = language,
            mainEntity = model.Faq.Items.Select(faq => new
            {
                @type = "Question",
                name = faq.Question,
                acceptedAnswer = new
                {
                    @type = "Answer",
                    text = faq.Answer
                }
            })
        };
    }

    private static string NormalizePathWithoutCulturePrefix(string path, IReadOnlyList<string> supportedCultures)
    {
        var normalized = string.IsNullOrWhiteSpace(path) ? "/" : path;
        if (!normalized.StartsWith('/'))
        {
            normalized = "/" + normalized;
        }

        var segments = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
        {
            return "/";
        }

        var first = segments[0];
        if (supportedCultures.Any(c => string.Equals(c, first, StringComparison.OrdinalIgnoreCase)))
        {
            if (segments.Length == 1)
            {
                return "/";
            }

            return "/" + string.Join('/', segments.Skip(1));
        }

        return normalized;
    }

    private static string BuildLocalizedUrl(string baseUrl, string normalizedPath, string culture, string defaultCulture)
    {
        var path = string.IsNullOrWhiteSpace(normalizedPath) ? "/" : normalizedPath;
        if (!path.StartsWith('/'))
        {
            path = "/" + path;
        }

        var localizedPath = string.Equals(culture, defaultCulture, StringComparison.OrdinalIgnoreCase)
            ? path
            : $"/{culture}{path}";

        return $"{baseUrl.TrimEnd('/')}{localizedPath}";
    }
}
