using Disqord;
using Disqord.Events;
using Hataraku.Bot.Entities;
using Hataraku.Bot.Entities.Commands;
using Hataraku.Bot.Entities.Commands.Parsers;
using Hataraku.Bot.Entities.Results;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qmmands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Hataraku.Bot.Services
{
    public partial class CommandService : IHostedService
    {
        private readonly ILogger<CommandService> _logger;
        private readonly Qmmands.CommandService _commandService;
        private readonly InteractionService _interactionService;
        private readonly BotConfig _config;
        private readonly DiscordClient _client;
        private readonly IServiceProvider _serviceProvider;
        private readonly LocalEmoji? _success;
        private readonly LocalEmoji? _failure;

        private IEnumerable<string> prefixes;
        
        public CommandService(ILogger<CommandService> logger, Qmmands.CommandService commandService, InteractionService interactionService, IOptions<BotConfig> config, DiscordClient client, IServiceProvider serviceProvider)
        {
            this._logger = logger;
            this._commandService = commandService;
            this._interactionService = interactionService;
            this._config = config.Value;
            this._client = client;
            this._serviceProvider = serviceProvider;
            
            if (this._config.EnableExecutionReaction)
            {
                try
                {
                    this._success = new LocalEmoji(this._config.SuccessEmoji);
                    this._failure = new LocalEmoji(this._config.FailureEmoji);
                }
                catch { }
            }

            this.prefixes = Enumerable.Empty<string>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._client.MessageReceived += (e) => { _ = OnMessageReceived(e); return Task.CompletedTask; };
            this._client.Ready += OnReady;

            this._commandService.AddModules(Assembly.GetEntryAssembly());
            this._commandService.AddTypeParser(new ModuleParser());
            this._commandService.AddTypeParser(new CommandParser());

            this._commandService.CommandExecuted += (e) => { _ = OnCommandExecuted(e); return Task.CompletedTask; };
            this._commandService.CommandExecutionFailed += (e) => { _ = OnCommandFailed(e); return Task.CompletedTask; };

            return Task.CompletedTask;
        }

        private Task OnReady(ReadyEventArgs e)
        {
            this.prefixes = this._config.EnableMention
                ? this._config.PrefixEnumerable.Concat(new[] { this._client.CurrentUser.Mention })
                : this._config.PrefixEnumerable;

            this._client.Ready -= OnReady;

            return Task.CompletedTask;
        }

        private Task OnCommandFailed(CommandExecutionFailedEventArgs e)
            => e.Context is HatarakuCommandContext hatarakuCommandContext
                ? HandleCommandFailed(hatarakuCommandContext, e.Result.Reason, e.Result.Exception)
                : Task.CompletedTask;

        private Task OnCommandExecuted(CommandExecutedEventArgs e)
            => !(e.Context is HatarakuCommandContext hatarakuCommandContext)
                ? Task.CompletedTask
                : e.Result is HatarakuFailedResult failed
                    ? HandleCommandFailed(hatarakuCommandContext, failed.Reason)
                    : e.Result is HatarakuSuccessResult success
                        ? HandleSuccessfulCommand(success, hatarakuCommandContext)
                        : HandleNonHatarakuResult(e.Result, hatarakuCommandContext);
                

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._client.MessageReceived -= OnMessageReceived;

            return Task.CompletedTask;
        }

        private async Task OnMessageReceived(MessageReceivedEventArgs eventArgs)
        {
            if (!(eventArgs.Message is CachedUserMessage userMessage)) return;
            if (userMessage.Author.IsBot) return;

            static ValueTask<(string Prefix, string Output)> FindPrefixAsync(CachedUserMessage message, IEnumerable<string> prefixes)
            {
                if (CommandUtilities.HasAnyPrefix(message.Content, prefixes, out string prefix, out string output))
                    return new ValueTask<(string Prefix, string Output)>((prefix, output));

                return default;
            }
            
            var prefixResult = await FindPrefixAsync(userMessage, prefixes);
            if (prefixResult == default) return;
            (string prefix, string output) = prefixResult;

            var context = new HatarakuCommandContext(this._serviceProvider, prefix, userMessage);
            var result = await this._commandService.ExecuteAsync(output, context);

            if (result is FailedResult failed) await HandleCommandFailed(context, failed.Reason);
        }
    }
}
