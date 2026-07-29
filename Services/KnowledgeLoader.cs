using System.Text.Json;
using MamiaSeedsOil.Web.Configuration;
using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Models.Knowledge;
using Microsoft.Extensions.Options;

namespace MamiaSeedsOil.Web.Services;

public sealed class KnowledgeLoader : IKnowledgeLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string[] PreferredFiles =
    [
        "company.json",
        "history.json",
        "products.json",
        "faq.json",
        "manufacturing.json",
        "contact.json",
        "gallery.json",
        "certifications.json",
        "distribution.json"
    ];

    private readonly AiAssistantOptions _options;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<KnowledgeLoader> _logger;

    public KnowledgeLoader(IOptions<AiAssistantOptions> options, IWebHostEnvironment environment, ILogger<KnowledgeLoader> logger)
    {
        _options = options.Value;
        _environment = environment;
        _logger = logger;
    }

    public async Task<CompanyKnowledgeModel> LoadAsync(CancellationToken cancellationToken = default)
    {
        var knowledgeFolderPath = Path.Combine(_environment.ContentRootPath, _options.KnowledgeFolderPath);
        if (Directory.Exists(knowledgeFolderPath))
        {
            var folderModel = await LoadFromFolderAsync(knowledgeFolderPath, cancellationToken);
            if (folderModel.Categories.Count > 0)
            {
                return folderModel;
            }
        }

        var path = Path.Combine(_environment.ContentRootPath, _options.KnowledgeFilePath);

        if (!File.Exists(path))
        {
            _logger.LogWarning("Knowledge base file does not exist at {Path}", path);
            return new CompanyKnowledgeModel();
        }

        await using var stream = File.OpenRead(path);
        var model = await JsonSerializer.DeserializeAsync<CompanyKnowledgeModel>(
            stream,
            JsonOptions,
            cancellationToken);

        return model ?? new CompanyKnowledgeModel();
    }

    private async Task<CompanyKnowledgeModel> LoadFromFolderAsync(string folderPath, CancellationToken cancellationToken)
    {
        var model = new CompanyKnowledgeModel
        {
            Version = "3.0",
            LastUpdatedUtc = DateTimeOffset.UtcNow
        };

        var seenCategoryKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var fileName in PreferredFiles)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filePath = Path.Combine(folderPath, fileName);
            if (!File.Exists(filePath))
            {
                continue;
            }

            await using var stream = File.OpenRead(filePath);
            var payload = await JsonSerializer.DeserializeAsync<KnowledgeFilePayload>(stream, JsonOptions, cancellationToken);

            if (payload is null)
            {
                continue;
            }

            if (payload.DomainKeywords is not null)
            {
                model.DomainKeywords.AddRange(payload.DomainKeywords.Where(v => !string.IsNullOrWhiteSpace(v)));
            }

            if (payload.CompanyInformation is not null)
            {
                model.CompanyInformation = payload.CompanyInformation;
            }

            if (payload.CompanyHistory is not null)
            {
                model.CompanyHistory = payload.CompanyHistory;
            }

            if (payload.Products is not null && payload.Products.Count > 0)
            {
                model.Products.AddRange(payload.Products);
            }

            if (payload.FrequentlyAskedQuestions is not null && payload.FrequentlyAskedQuestions.Count > 0)
            {
                model.FrequentlyAskedQuestions.AddRange(payload.FrequentlyAskedQuestions);
            }

            if (payload.ContactInformation is not null)
            {
                model.ContactInformation = payload.ContactInformation;
            }

            if (payload.FutureDocuments is not null && payload.FutureDocuments.Count > 0)
            {
                model.FutureDocuments.AddRange(payload.FutureDocuments);
            }

            if (payload.Category is not null && !string.IsNullOrWhiteSpace(payload.Category.Key) && seenCategoryKeys.Add(payload.Category.Key))
            {
                payload.Category.Articles = payload.Entries ?? [];
                model.Categories.Add(payload.Category);
            }
        }

        model.DomainKeywords = model.DomainKeywords
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return model;
    }

    private sealed class KnowledgeFilePayload
    {
        public List<string>? DomainKeywords { get; set; }
        public CompanyInformation? CompanyInformation { get; set; }
        public HistoryKnowledge? CompanyHistory { get; set; }
        public List<ProductKnowledge>? Products { get; set; }
        public List<FAQModel>? FrequentlyAskedQuestions { get; set; }
        public ContactKnowledge? ContactInformation { get; set; }
        public KnowledgeCategory? Category { get; set; }
        public List<KnowledgeArticle>? Entries { get; set; }
        public List<KnowledgeArticle>? FutureDocuments { get; set; }
    }
}
