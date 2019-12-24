using Disqord;
using Hataraku.Bot.Entities.Commands;
using Hataraku.Bot.Entities.Commands.Interactivity;
using Hataraku.Bot.Entities.Results;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Qmmands;
using System;
using System.Threading.Tasks;

namespace Hataraku.Bot.Services
{
    public partial class CommandService : IHostedService
    {
        private Task HandleNonHatarakuResult(CommandResult result, HatarakuCommandContext context)
        {
            if (result.IsSuccessful)
                return ReactAsync(true, context);
            else if (result is HatarakuFailedResult failed && !string.IsNullOrEmpty(failed.Reason))
                return HandleCommandFailed(context, failed.Reason);

            return HandleCommandFailed(context);
        }

        private async Task HandleSuccessfulCommand(HatarakuSuccessResult result, HatarakuCommandContext context)
        {
            (string? message, LocalEmbed? embed, InteractiveMessageBuilder? interactiveMessage, Func<IUserMessage, Task>? continueWith) = result switch
            {
                HatarakuContinuedExecutionResult execution => (execution.Message, execution.Embed, execution.InteractiveMessageBuilder, execution.ContinueWith),
                HatarakuReplyResult reply => (reply.Message, reply.Embed, reply.InteractiveMessageBuilder, null),
                _ => (null, null, null, null)
            };

            await ReactAsync(true, context);

            IUserMessage? res = default;

            if (message is null && embed is null && interactiveMessage is null) return;

            if (!string.IsNullOrEmpty(message) || embed != null)
                res = await context.Channel.SendMessageAsync(message, embed: embed);

            if (interactiveMessage != null)
            {
                if (res == null) res = await context.Channel.SendMessageAsync(embed: new LocalEmbedBuilder().Build());

                await this._interactionService.WaitForAsync(interactiveMessage.WithMessage(res).Build());
            }

            if (res == null) return;

            if (continueWith != null) await continueWith(res);
        }

        private async Task HandleCommandFailed(HatarakuCommandContext context, string? reason = null, Exception? exception = null)
        {
            await ReactAsync(false, context);
            await context.Channel.SendMessageAsync(reason);
        }

        public Task ReactAsync(bool success, HatarakuCommandContext context)
        {
            var emoji = success ? this._success : this._failure;

            return emoji is null ? Task.CompletedTask : context.Message.AddReactionAsync(emoji);
        }
    }
}
