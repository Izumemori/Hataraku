using Disqord;
using Disqord.Events;
using Hataraku.Bot.Entities.Commands.Interactivity;
using Microsoft.Extensions.Logging;
using Qommon.Events;
using System;
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

        public async Task<T?> WaitForAsync<T>(IInteractiveMessage<T> interactiveMessage, int? timeoutMs = null)
            where T: DiscordEventArgs
        {
            var timeout = timeoutMs ?? DefaultTimeout;

            Func<Task> delay = async () =>
            {
                while (timeout > 0)
                {
                    await Task.Delay(100);
                    timeout -= 100;
                }
            };

            async Task EventHandler(DiscordEventArgs e)
            {
                if (!(e is T eventArgs)) return;

                if (!interactiveMessage.Precondition(eventArgs)) return;

                var timeout = timeoutMs ?? DefaultTimeout;

                await interactiveMessage.HandleEventArgsAsync(eventArgs);
            }

            await interactiveMessage.SetupAsync();

            this._client.ReactionAdded += EventHandler;
            this._client.MessageReceived += EventHandler;

            var task = interactiveMessage.TaskCompletionSource.Task;

            var taskOrDelay = await Task.WhenAny(task, delay());

            this._client.ReactionAdded -= EventHandler;
            this._client.MessageReceived -= EventHandler;

            await interactiveMessage.DisposeAsync();

            return taskOrDelay == task
                ? await task
                : default;
        }
    }
}
