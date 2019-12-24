using Disqord;
using Disqord.Events;
using Hataraku.Bot.Entities.Commands.Interactivity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hataraku.Bot.Services
{
    public class InteractionService
    {
        private const int DefaultTimeout = 30_000;

        private readonly ILogger<InteractionService> _logger;
        private readonly DiscordClient _client;

        public InteractionService(ILogger<InteractionService> logger, DiscordClient client)
        {
            this._client = client;
            this._logger = logger;
        }

        public async Task<IUserMessage?> WaitForAsync(IInteractiveMessage interactiveMessage, int? timeoutMs = null)
        {
            var timeout = timeoutMs ?? DefaultTimeout;

            var cancelToken = new CancellationTokenSource();

            async Task EventHandler(DiscordEventArgs e)
            {
                if (!interactiveMessage.Precondition(e)) return;

                if (!await interactiveMessage.HandleEventArgsAsync(e)) return;

                cancelToken.CancelAfter(timeout);
            }

            await interactiveMessage.SetupAsync();

            this._client.ReactionAdded += EventHandler;
            this._client.MessageReceived += EventHandler;

            var task = interactiveMessage.TaskCompletionSource.Task;
            var delay = Task.Delay(-1, cancelToken.Token);
            cancelToken.CancelAfter(timeout);

            var taskOrDelay = await Task.WhenAny(task, delay);

            this._client.ReactionAdded -= EventHandler;
            this._client.MessageReceived -= EventHandler;

            await interactiveMessage.DisposeAsync();

            return taskOrDelay == task
                ? await task
                : default;
        }
    }
}
