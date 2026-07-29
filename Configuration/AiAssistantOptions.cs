using System.ComponentModel.DataAnnotations;

namespace MamiaSeedsOil.Web.Configuration;

public sealed class AiAssistantOptions
{
    public const string SectionName = "AiAssistant";

    public bool Enabled { get; set; } = true;

    [Required]
    public string DisplayName { get; set; } = "Mamia Assistant";

    [Required]
    public string WelcomeMessage { get; set; } = "Welcome to Mamia Seeds Oil Limited. I'm Mamia Assistant. I can help you with our products, distribution, manufacturing process, certifications and general company information. How may I assist you today?";

    [Required]
    public string KnowledgeFilePath { get; set; } = "Configuration/ai-knowledge-base.json";

    [Required]
    public string KnowledgeFolderPath { get; set; } = "Data/Knowledge";

    [Required]
    public string NotFoundResponse { get; set; } = "I can only help with Mamia Seeds Oil information. Please contact our team for anything outside that scope.";

    [Required]
    public string ContactFallbackResponse { get; set; } = "I do not have that specific detail. Please contact our sales team via phone, email, or the contact form for accurate assistance.";

    [Required]
    public string RestrictionResponse { get; set; } = "I can only discuss Mamia Seeds Oil products, operations, and business information.";

    [Required]
    public string UnavailableInformationResponse { get; set; } = "This information is currently unavailable. Please contact Mamia Seeds Oil Limited directly for further assistance.";

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
