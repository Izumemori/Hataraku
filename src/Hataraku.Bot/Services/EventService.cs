using Disqord;
using Disqord.Events;
using Disqord.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Hataraku.Bot.Services
{
    public class EventService : IHostedService
    {
        private readonly ILogger<EventService> _logger;
        private readonly DiscordClient _client;

        public EventService(ILogger<EventService> logger, DiscordClient client)
        {
            this._logger = logger;
            this._client = client;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._client.Logger.MessageLogged += OnLogMessage;
            this._client.Ready += OnReady;

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._client.Logger.MessageLogged -= OnLogMessage;

            return Task.CompletedTask;
        }

        private void OnLogMessage(object? _, MessageLoggedEventArgs eventArgs)
            => this._logger.Log((LogLevel)eventArgs.Severity, eventArgs.Exception, eventArgs.Message);

        private Task OnReady(ReadyEventArgs eventArgs)
        {
            this._logger.LogInformation("Logged in as {CurrentUser}", eventArgs.Client.CurrentUser);

            return Task.CompletedTask;
        }
    }
}
