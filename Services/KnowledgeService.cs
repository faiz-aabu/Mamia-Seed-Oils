using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Models.Knowledge;
using MamiaSeedsOil.Web.Models.KnowledgeArchitecture;
using ArchitectureKnowledgeCategory = MamiaSeedsOil.Web.Models.KnowledgeArchitecture.KnowledgeCategory;
using StructuredKnowledgeCategory = MamiaSeedsOil.Web.Models.Knowledge.KnowledgeCategory;

namespace MamiaSeedsOil.Web.Services;

public sealed class KnowledgeService : IKnowledgeService
{
    private static readonly string[] BaselineDomainKeywords =
    [
        "mamia",
        "mamia seeds oil",
        "mamia seeds oil limited",
        "soybean",
        "soya oil",
        "kaduna",
        "makarfi",
        "company",
        "products",
        "history",
        "faq",
        "contact",
        "manufacturing",
        "distribution",
        "gallery",
        "certifications"
    ];

    private readonly IKnowledgeRepository _knowledgeRepository;

    public KnowledgeService(IKnowledgeRepository knowledgeRepository)
    {
        _knowledgeRepository = knowledgeRepository;
    }

    public async Task<IReadOnlyList<ArchitectureKnowledgeCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var model = await _knowledgeRepository.GetAsync(cancellationToken);
        return BuildCategories(model);
    }

    public async Task<IReadOnlyList<KnowledgeEntry>> GetEntriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await GetCategoriesAsync(cancellationToken);
        return categories.SelectMany(c => c.Entries).ToList();
    }

    public bool IsDomainQuestion(string question)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return false;
        }

        var normalized = Normalize(question);
        return BaselineDomainKeywords.Any(keyword => normalized.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    private static IReadOnlyList<ArchitectureKnowledgeCategory> BuildCategories(CompanyKnowledgeModel model)
    {
        var categories = new List<ArchitectureKnowledgeCategory>();

        foreach (StructuredKnowledgeCategory category in model.Categories)
        {
            categories.Add(new ArchitectureKnowledgeCategory
            {
                Key = category.Key,
                Title = category.Title,
                Description = category.Description,
                Keywords = category.Keywords,
                Entries = category.Articles.Select(article => new KnowledgeEntry
                {
                    Id = article.Id,
                    CategoryKey = category.Key,
                    CategoryTitle = category.Title,
                    Title = article.Title,
                    Content = article.Content,
                    Keywords = article.Keywords,
                    Source = new KnowledgeSource
                    {
                        SourceType = ParseSourceType(article.SourceType),
                        SourcePath = article.SourcePath,
                        SourceId = article.Id
                    }
                }).ToList()
            });
        }

        // Ensure FAQ participates in category-scoped search.
        categories.Add(new ArchitectureKnowledgeCategory
        {
            Key = "faq",
            Title = "FAQ",
            Description = "Frequently asked questions",
            Keywords = ["faq", "questions", "answers"],
            Entries = model.FrequentlyAskedQuestions.Select((faq, index) => new KnowledgeEntry
            {
                Id = $"faq-{index + 1}",
                CategoryKey = "faq",
                CategoryTitle = "FAQ",
                Title = faq.Question,
                Content = faq.Answer,
                Keywords = faq.Keywords,
                Source = new KnowledgeSource
                {
                    SourceType = KnowledgeSourceType.Json,
                    SourcePath = "Data/Knowledge/faq.json",
                    SourceId = $"faq-{index + 1}"
                }
            }).ToList()
        });

        return categories;
    }

    private static KnowledgeSourceType ParseSourceType(string sourceType)
    {
        if (string.IsNullOrWhiteSpace(sourceType))
        {
            return KnowledgeSourceType.Json;
        }

        return sourceType.Trim().ToUpperInvariant() switch
        {
            "PDF" => KnowledgeSourceType.Pdf,
            "DOCX" or "WORD" => KnowledgeSourceType.Docx,
            "TXT" => KnowledgeSourceType.Txt,
            "MD" or "MARKDOWN" => KnowledgeSourceType.Markdown,
            "XLS" or "XLSX" or "EXCEL" => KnowledgeSourceType.Excel,
            "KNOWLEDGEBASE" or "JSON" => KnowledgeSourceType.Json,
            _ => KnowledgeSourceType.Other
        };
    }

    private static string Normalize(string input)
    {
        return input.Trim().ToLowerInvariant();
    }
}
