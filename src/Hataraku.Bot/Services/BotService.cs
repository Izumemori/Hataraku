using Disqord;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Hataraku.Bot.Services
{
    public class BotService : IHostedService
    {
        private readonly ILogger<BotService> _logger;
        private readonly DiscordClient _client;

        public BotService(ILogger<BotService> logger, DiscordClient client)
        {
            this._logger = logger;
            this._client = client;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._logger.LogDebug("Starting Bot...");
            _ = this._client.RunAsync();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._logger.LogDebug("Stopping Bot...");
            return this._client.StopAsync();
        }
    }
}
