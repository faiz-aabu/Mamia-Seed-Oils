using MamiaSeedsOil.Web.Configuration;
using MamiaSeedsOil.Web.Interfaces;
using Microsoft.Extensions.Options;

namespace MamiaSeedsOil.Web.Services;

public sealed class AiProviderFactory : IAiProviderFactory
{
    private readonly ProviderOptions _providerOptions;
    private readonly IEnumerable<IAiProvider> _providers;

    public AiProviderFactory(IOptions<AiAssistantOptions> options, IEnumerable<IAiProvider> providers)
    {
        _providerOptions = options.Value.Provider;
        _providers = providers;
    }

    public IAiProvider ResolveProvider(string? providerName = null)
    {
        var desired = string.IsNullOrWhiteSpace(providerName) ? _providerOptions.DefaultProvider : providerName;
        var match = _providers.FirstOrDefault(p => string.Equals(p.ProviderName, desired, StringComparison.OrdinalIgnoreCase));
        return match ?? _providers.First(p => p.ProviderName.Equals("RuleBased", StringComparison.OrdinalIgnoreCase));
    }
}
