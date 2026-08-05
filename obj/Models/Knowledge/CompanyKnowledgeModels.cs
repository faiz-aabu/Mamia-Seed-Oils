namespace MamiaSeedsOil.Web.Models.Knowledge;

public sealed class CompanyKnowledgeModel
{
    public string Version { get; set; } = "1.0";
    public DateTimeOffset LastUpdatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public CompanyInformation CompanyInformation { get; set; } = new();
    public HistoryKnowledge CompanyHistory { get; set; } = new();
    public List<ProductKnowledge> Products { get; set; } = [];
    public List<FAQModel> FrequentlyAskedQuestions { get; set; } = [];
    public ContactKnowledge ContactInformation { get; set; } = new();
    public List<KnowledgeCategory> Categories { get; set; } = [];
    public List<KnowledgeArticle> News { get; set; } = [];
    public List<KnowledgeArticle> GalleryDescriptions { get; set; } = [];
    public List<KnowledgeArticle> FutureDocuments { get; set; } = [];
    public RetrievalMetadata Retrieval { get; set; } = new();
    public List<string> DomainKeywords { get; set; } = [];
}

public sealed class RetrievalMetadata
{
    public string Strategy { get; set; } = "keyword-search";
    public string VectorIndexName { get; set; } = "[To Be Updated]";
    public string EmbeddingModel { get; set; } = "[To Be Updated]";
    public string ChunkingPolicy { get; set; } = "[To Be Updated]";
}

public sealed class CompanyInformation
{
    public string CompanyName { get; set; } = "[To Be Updated]";
    public string EstablishedDate { get; set; } = "[To Be Updated]";
    public List<string> LocationLines { get; set; } = [];
    public string BusinessDescription { get; set; } = "[To Be Updated]";
    public string AdditionalProductDescription { get; set; } = "[To Be Updated]";
}

public sealed class HistoryKnowledge
{
    public string Overview { get; set; } = "[To Be Updated]";
    public List<KnowledgeArticle> Timeline { get; set; } = [];
}

public sealed class ProductKnowledge
{
    public string Name { get; set; } = "[To Be Updated]";
    public string Description { get; set; } = "[To Be Updated]";
    public List<string> PackagingSizes { get; set; } = [];
    public string Availability { get; set; } = "[To Be Updated]";
    public string Category { get; set; } = "[To Be Updated]";
    public string AdditionalNotes { get; set; } = "[To Be Updated]";
}

public sealed class ContactKnowledge
{
    public string Phone { get; set; } = "+234 806 444 4142 / +234 803 507 1248";
    public string Email { get; set; } = "info@mamiaseedsoil.com";
    public List<string> AddressLines { get; set; } = [];
    public string BusinessHours { get; set; } = "Monday - Saturday, 8:00am - 6:00pm";
    public string WhatsApp { get; set; } = "https://wa.me/2348064444142";
}

public sealed class FAQModel
{
    public string Question { get; set; } = "[To Be Updated]";
    public string Answer { get; set; } = "[To Be Updated]";
    public List<string> Keywords { get; set; } = [];
}

public sealed class KnowledgeCategory
{
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Keywords { get; set; } = [];
    public List<KnowledgeArticle> Articles { get; set; } = [];
}

public sealed class KnowledgeArticle
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = "[To Be Updated]";
    public List<string> Keywords { get; set; } = [];
    public string SourceType { get; set; } = "KnowledgeBase";
    public string SourcePath { get; set; } = string.Empty;
}

public sealed class KnowledgeSearchResult
{
    public bool IsDomainQuery { get; set; }
    public bool HasMatch { get; set; }
    public bool IsUnavailable { get; set; }
    public string ResponseText { get; set; } = string.Empty;
    public string MatchedCategory { get; set; } = string.Empty;
    public string MatchedArticleTitle { get; set; } = string.Empty;
    public double Score { get; set; }
}

public sealed class KnowledgeValidationResult
{
    public bool IsValid { get; set; }
    public IReadOnlyList<string> Warnings { get; init; } = [];
}
