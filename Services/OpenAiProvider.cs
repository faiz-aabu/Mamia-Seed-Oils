using MamiaSeedsOil.Web.Configuration;
using MamiaSeedsOil.Web.Interfaces;
using MamiaSeedsOil.Web.Models.AiAssistant;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MamiaSeedsOil.Web.Services;

public sealed class OpenAiProvider : IAiProvider
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly AiAssistantOptions _options;
    private readonly IAIContextBuilder _contextBuilder;
    private readonly IAIResponseBuilder _responseBuilder;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<OpenAiProvider> _logger;

    public OpenAiProvider(
        IOptions<AiAssistantOptions> options,
        IAIContextBuilder contextBuilder,
        IAIResponseBuilder responseBuilder,
        IHttpClientFactory httpClientFactory,
        ILogger<OpenAiProvider> logger)
    {
        _options = options.Value;
        _contextBuilder = contextBuilder;
        _responseBuilder = responseBuilder;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public string ProviderName => "OpenAI";

    public async Task<AiAnswerResult> GenerateAnswerAsync(string message, CancellationToken cancellationToken = default)
    {
        var fallback = await BuildFallbackAnswerAsync(message, cancellationToken);
        var providerSettings = _options.Provider.OpenAI;

        if (!_options.Enabled || !providerSettings.Enabled || string.IsNullOrWhiteSpace(providerSettings.ApiKey) || string.IsNullOrWhiteSpace(providerSettings.Model))
        {
            return fallback;
        }

        try
        {
            using var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(20);

            var requestUri = ResolveEndpoint(providerSettings.Endpoint);
            var requestBody = new
            {
                model = providerSettings.Model,
                temperature = 0.2,
                max_tokens = 250,
                messages = new object[]
                {
                    new { role = "system", content = BuildSystemPrompt() },
                    new { role = "user", content = message }
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = JsonContent.Create(requestBody)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", providerSettings.ApiKey);

            using var response = await client.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("OpenAI request failed. StatusCode={StatusCode}", response.StatusCode);
                return fallback;
            }

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var payload = await JsonSerializer.DeserializeAsync<OpenAiChatResponse>(stream, JsonOptions, cancellationToken);
            var responseText = payload?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();

            if (string.IsNullOrWhiteSpace(responseText))
            {
                return fallback;
            }

            return new AiAnswerResult
            {
                Message = SanitizeResponse(responseText),
                IsFallback = false,
                Suggestions = fallback.Suggestions
            };
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "OpenAI provider failed. Falling back to local knowledge response.");
            return fallback;
        }
    }

    public async IAsyncEnumerable<string> StreamAnswerAsync(string message, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var result = await GenerateAnswerAsync(message, cancellationToken);
        var words = result.Message.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in words)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return word + " ";
            await Task.Delay(16, cancellationToken);
        }
    }

    private async Task<AiAnswerResult> BuildFallbackAnswerAsync(string message, CancellationToken cancellationToken)
    {
        var context = await _contextBuilder.BuildAsync(message, cancellationToken);
        return _responseBuilder.Build(context, _options.SuggestedQuestions);
    }

    private static string ResolveEndpoint(string configuredEndpoint)
    {
        var endpoint = string.IsNullOrWhiteSpace(configuredEndpoint)
            ? "https://api.openai.com/v1/chat/completions"
            : configuredEndpoint.Trim();

        if (endpoint.Contains("/chat/completions", StringComparison.OrdinalIgnoreCase))
        {
            return endpoint;
        }

        if (endpoint.EndsWith("/v1", StringComparison.OrdinalIgnoreCase))
        {
            return endpoint + "/chat/completions";
        }

        return endpoint.TrimEnd('/') + "/chat/completions";
    }

    private static string BuildSystemPrompt()
    {
        return """
You are the official AI assistant for Mamia Seeds Oil Limited.

Answer only questions about the company, its products, location, history, manufacturing, certifications, contact information, and distributor opportunities.
If the user asks about anything unrelated to Mamia Seeds Oil Limited, politely explain that you can only help with company-related enquiries.

Keep your answers polite, concise, and helpful.

Company facts:
- Company: Mamia Seeds Oil Limited
- Established: 26 July 2005
- Location: Kutungare Village, Airport Road, Makarfi, Kaduna State, Nigeria
- Products: 4L Bottle, 4L Jerry Can, 10L Jerry Can, 20L Jerry Can, Refined Soya Cooking Oil, Industrial Bulk Supply, Soybean By-products
- Business: Large-scale soybean processing company that processes soybeans into refined cooking oil and produces soybean meal and by-products for wholesale and industrial customers across Nigeria
- Contact: info@mamiaseedsoil.com | +234 806 444 4142 | +234 803 507 1248
- Instagram: https://www.instagram.com/mamiaseedsoilslimited/

When asked about products, recommend the most relevant product based on the request. When asked about becoming a distributor, explain that the company welcomes distributor enquiries and provide the contact information. When asked about soybean processing, explain it in simple, clear terms.
""";
    }

    private static string SanitizeResponse(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return string.Empty;
        }

        var sanitized = content.Trim();
        sanitized = sanitized.Replace("```", string.Empty);
        return sanitized;
    }

    private sealed class OpenAiChatResponse
    {
        public List<OpenAiChoice>? Choices { get; set; }
    }

    private sealed class OpenAiChoice
    {
        public OpenAiMessage? Message { get; set; }
    }

    private sealed class OpenAiMessage
    {
        public string? Content { get; set; }
    }
}
