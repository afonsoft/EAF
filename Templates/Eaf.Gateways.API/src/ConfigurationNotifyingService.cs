using Ocelot.Configuration.ChangeTracking;

namespace Eaf.Gateways.API
{
    public class ConfigurationNotifyingService : BackgroundService
    {
        private readonly IOcelotConfigurationChangeTokenSource _tokenSource;
        private readonly ILogger<ConfigurationNotifyingService> _logger;

        public ConfigurationNotifyingService(IOcelotConfigurationChangeTokenSource tokenSource, ILogger<ConfigurationNotifyingService> logger)
        {
            _tokenSource = tokenSource;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_tokenSource.ChangeToken.HasChanged)
                {
                    _logger.LogInformation("Configuration updated");
                }
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}