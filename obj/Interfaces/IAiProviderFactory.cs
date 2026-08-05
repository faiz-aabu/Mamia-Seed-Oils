namespace MamiaSeedsOil.Web.Interfaces;

public interface IAiProviderFactory
{
    IAiProvider ResolveProvider(string? providerName = null);
}
