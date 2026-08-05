using System.Text.RegularExpressions;
using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Models.Knowledge;

namespace MamiaSeedsOil.Web.Services;

public sealed class KnowledgeSearchService : IKnowledgeSearchService
{
    private static readonly Regex MultiSpaceRegex = new("\\s+", RegexOptions.Compiled);

    public Task<KnowledgeSearchResult> SearchAsync(string query, CompanyKnowledgeModel knowledgeBase, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var normalizedQuery = Normalize(query);
        var domainKeywords = BuildDomainKeywords(knowledgeBase);

        var isDomainQuery = domainKeywords.Any(keyword => normalizedQuery.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        if (!isDomainQuery)
        {
            return Task.FromResult(new KnowledgeSearchResult
            {
                IsDomainQuery = false,
                HasMatch = false,
                IsUnavailable = false,
                ResponseText = string.Empty
            });
        }

        var allEntries = Flatten(knowledgeBase);
        var match = allEntries
            .Select(entry => new
            {
                Entry = entry,
                Score = Score(normalizedQuery, entry.Keywords)
            })
            .OrderByDescending(x => x.Score)
            .FirstOrDefault();

        if (match is null || match.Score <= 0)
        {
            return Task.FromResult(new KnowledgeSearchResult
            {
                IsDomainQuery = true,
                HasMatch = false,
                IsUnavailable = false,
                ResponseText = string.Empty
            });
        }

        var unavailable = string.IsNullOrWhiteSpace(match.Entry.Content) || string.Equals(match.Entry.Content.Trim(), "[To Be Updated]", StringComparison.OrdinalIgnoreCase);

        return Task.FromResult(new KnowledgeSearchResult
        {
            IsDomainQuery = true,
            HasMatch = true,
            IsUnavailable = unavailable,
            ResponseText = match.Entry.Content,
            MatchedCategory = match.Entry.Category,
            MatchedArticleTitle = match.Entry.Title,
            Score = match.Score
        });
    }

    private static IEnumerable<string> BuildDomainKeywords(CompanyKnowledgeModel knowledgeBase)
    {
        var baseline = new[]
        {
            "mamia", "mamia seeds oil", "mamia seeds oil limited", "soyabean", "soybean", "soya oil", "kaduna", "makarfi"
        };

        var categoryKeywords = knowledgeBase.Categories
            .SelectMany(category => category.Keywords
                .Concat(category.Articles.SelectMany(article => article.Keywords)));

        var faqKeywords = knowledgeBase.FrequentlyAskedQuestions
            .SelectMany(faq => faq.Keywords);

        var productKeywords = knowledgeBase.Products
            .SelectMany(product => new[] { product.Name, product.Category }
                .Concat(product.PackagingSizes));

        var contactKeywords = new[]
        {
            "contact", "phone", "email", "address", "business hours", "hours", "whatsapp"
        };

        return baseline
            .Concat(knowledgeBase.DomainKeywords ?? [])
            .Concat(categoryKeywords)
            .Concat(faqKeywords)
            .Concat(productKeywords)
            .Concat(contactKeywords)
            .Select(Normalize)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase);
    }

    private static IEnumerable<KnowledgeFlatEntry> Flatten(CompanyKnowledgeModel kb)
    {
        var entries = new List<KnowledgeFlatEntry>();

        entries.Add(new KnowledgeFlatEntry(
            "Company Information",
            "Company Information",
            $"{kb.CompanyInformation.CompanyName}. Established {kb.CompanyInformation.EstablishedDate}. Location: {string.Join(", ", kb.CompanyInformation.LocationLines)}. {kb.CompanyInformation.BusinessDescription} {kb.CompanyInformation.AdditionalProductDescription}",
            ["company", "information", "name", "established", "location", "business", "by-products"]));

        entries.Add(new KnowledgeFlatEntry(
            "Company History",
            "Company History Overview",
            kb.CompanyHistory.Overview,
            ["history", "origin", "background", "timeline", "founded"]));

        entries.AddRange(kb.CompanyHistory.Timeline.Select(item =>
            new KnowledgeFlatEntry("Company Timeline", item.Title, item.Content, item.Keywords)));

        entries.AddRange(kb.Products.Select(product =>
            new KnowledgeFlatEntry(
                "Products",
                product.Name,
                $"{product.Description} Packaging: {string.Join(", ", product.PackagingSizes)}. Availability: {product.Availability}. Category: {product.Category}. {product.AdditionalNotes}",
                ["products", "packaging", "sizes", product.Name, product.Category])));

        entries.AddRange(kb.FrequentlyAskedQuestions.Select(faq =>
            new KnowledgeFlatEntry("Frequently Asked Questions", faq.Question, faq.Answer, faq.Keywords)));

        entries.Add(new KnowledgeFlatEntry(
            "Contact Information",
            "Contact",
            $"Phone: {kb.ContactInformation.Phone}. Email: {kb.ContactInformation.Email}. Address: {string.Join(", ", kb.ContactInformation.AddressLines)}. Hours: {kb.ContactInformation.BusinessHours}. WhatsApp: {kb.ContactInformation.WhatsApp}",
            ["contact", "phone", "email", "address", "business hours", "whatsapp"]));

        entries.AddRange(kb.Categories.SelectMany(category =>
            category.Articles.Select(article =>
                new KnowledgeFlatEntry(category.Title, article.Title, article.Content, category.Keywords.Concat(article.Keywords)))));

        entries.AddRange(kb.News.Select(news =>
            new KnowledgeFlatEntry("News", news.Title, news.Content, news.Keywords)));

        entries.AddRange(kb.GalleryDescriptions.Select(gallery =>
            new KnowledgeFlatEntry("Gallery Descriptions", gallery.Title, gallery.Content, gallery.Keywords)));

        entries.AddRange(kb.FutureDocuments.Select(doc =>
            new KnowledgeFlatEntry("Future Documents", doc.Title, doc.Content, doc.Keywords.Concat(new[] { doc.SourceType, doc.SourcePath }))));

        return entries;
    }

    private static double Score(string normalizedQuery, IEnumerable<string> keywords)
    {
        var keywordList = keywords
            .Select(Normalize)
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (keywordList.Length == 0)
        {
            return 0;
        }

        var hits = keywordList.Count(keyword => normalizedQuery.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        return (double)hits / keywordList.Length;
    }

    private static string Normalize(string value)
    {
        var lowered = (value ?? string.Empty).Trim().ToLowerInvariant();
        return MultiSpaceRegex.Replace(lowered, " ");
    }

    private sealed record KnowledgeFlatEntry(string Category, string Title, string Content, IEnumerable<string> Keywords);
}
