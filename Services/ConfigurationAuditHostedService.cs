using MamiaSeedsOil.Web.Configuration;
using Microsoft.Extensions.Options;

namespace MamiaSeedsOil.Web.Services;

public sealed class ConfigurationAuditHostedService : IHostedService
{
    private readonly ILogger<ConfigurationAuditHostedService> _logger;
    private readonly EmailDeliveryOptions _emailOptions;
    private readonly FutureDataConnectionsOptions _dataOptions;

    public ConfigurationAuditHostedService(
        ILogger<ConfigurationAuditHostedService> logger,
        IOptions<EmailDeliveryOptions> emailOptions,
        IOptions<FutureDataConnectionsOptions> dataOptions)
    {
        _logger = logger;
        _emailOptions = emailOptions.Value;
        _dataOptions = dataOptions.Value;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(_emailOptions.Provider) || _emailOptions.Provider.Contains("[To Be Updated]", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Configuration issue: EmailDelivery.Provider is not fully configured.");
        }

        if (string.IsNullOrWhiteSpace(_dataOptions.Provider) || _dataOptions.Provider.Contains("[To Be Updated]", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Configuration issue: FutureDataConnections.Provider is not fully configured.");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}
