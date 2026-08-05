using System.ComponentModel.DataAnnotations;

namespace MamiaSeedsOil.Web.Configuration;

public sealed class AiAssistantOptions
{
    public const string SectionName = "AiAssistant";

    public bool Enabled { get; set; } = true;

    [Required]
    public string DisplayName { get; set; } = "Mamia Assistant";

    [Required]
    public string WelcomeMessage { get; set; } = "Welcome to Mamia Seeds Oil Limited. I'm here to answer questions about our products, bulk orders, soyabean meal, cooking oil, certifications, factory operations, distribution and partnerships.";

    [Required]
    public string KnowledgeFilePath { get; set; } = "Configuration/ai-knowledge-base.json";

    [Required]
    public string KnowledgeFolderPath { get; set; } = "Data/Knowledge";

    [Required]
    public string NotFoundResponse { get; set; } = "I don't have that information at the moment. Please contact our team through the Contact page for further assistance.";

    [Required]
    public string ContactFallbackResponse { get; set; } = "I don't have that information at the moment. Please contact our team through the Contact page for further assistance.";

    [Required]
    public string RestrictionResponse { get; set; } = "I don't have that information at the moment. Please contact our team through the Contact page for further assistance.";

    [Required]
    public string UnavailableInformationResponse { get; set; } = "I don't have that information at the moment. Please contact our team through the Contact page for further assistance.";

    public List<string> SuggestedQuestions { get; set; } = [];

    [Required]
    public ProviderOptions Provider { get; set; } = new();

    [Required]
    public RateLimitOptions RateLimit { get; set; } = new();
}

public sealed class ProviderOptions
{
    [Required]
    public string DefaultProvider { get; set; } = "RuleBased";

    public ProviderConnectionOptions OpenAI { get; set; } = new();
    public ProviderConnectionOptions AzureOpenAI { get; set; } = new();
    public ProviderConnectionOptions Gemini { get; set; } = new();
    public ProviderConnectionOptions Claude { get; set; } = new();
    public ProviderConnectionOptions OpenRouter { get; set; } = new();
    public ProviderConnectionOptions Ollama { get; set; } = new();

    public bool EnableStreamingEndpoint { get; set; } = true;
}

public sealed class ProviderConnectionOptions
{
    public bool Enabled { get; set; }
    public string Model { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}

public sealed class RateLimitOptions
{
    [Range(1, 1000)]
    public int PermitLimit { get; set; } = 20;

    [Range(1, 3600)]
    public int WindowSeconds { get; set; } = 60;

    [Range(0, 1000)]
    public int QueueLimit { get; set; } = 0;
}
