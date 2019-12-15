using Disqord;
using Hataraku.Bot.Entities.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Qmmands;
using System;
using System.Threading.Tasks;

namespace Hataraku.Bot.Entities.Commands
{
    public abstract class HatarakuModuleBase : HatarakuModuleBase<HatarakuCommandContext>
    { }

    public class HatarakuModuleBase<T> : ModuleBase<T>
        where T: HatarakuCommandContext
    {
        private ILogger<HatarakuModuleBase<T>> logger => Context.ServiceProvider.GetRequiredService<ILogger<HatarakuModuleBase<T>>>();

        protected override ValueTask BeforeExecutedAsync()
        {
            this.logger.LogDebug($"Executing {Context.Command} by {Context.User} in {FormatChannel()}");

            return default;
        }

        protected override ValueTask AfterExecutedAsync()
        {
            this.logger.LogDebug($"Executed {Context.Command} by {Context.User} in {FormatChannel()}");

            return default;
        }

        private string FormatChannel()
            => Context.Guild != null
                ? $"{Context.Channel} ({Context.Channel.Id}); Guild: {Context.Guild} ({Context.Guild.Id})"
                : $"{Context.Channel} ({Context.Channel.Id})";

        protected HatarakuCommandResult Ok(string message)
            => InternalOk(message, null);

        protected HatarakuCommandResult Ok(LocalEmbed embed)
            => InternalOk(null, embed);

        protected HatarakuCommandResult Ok(string message, LocalEmbed embed)
            => InternalOk(message, embed);

        protected HatarakuCommandResult Ok(string message, Func<IUserMessage, HatarakuCommandContext, Task> continueWith)
            => InternalOk(message, null, continueWith);

        protected HatarakuCommandResult Ok(LocalEmbed embed, Func<IUserMessage, HatarakuCommandContext, Task> continueWith)
            => InternalOk(null, embed, continueWith);

        protected HatarakuCommandResult Ok(string message, LocalEmbed embed, Func<IUserMessage, HatarakuCommandContext, Task> continueWith)
            => InternalOk(message, embed, continueWith);

        protected HatarakuCommandResult Ok<TState>(string message, TState state, Func<IUserMessage, HatarakuCommandContext, TState, Task> continueWith)
            => InternalOk(message, null, state, continueWith);

        protected HatarakuCommandResult Ok<TState>(LocalEmbed embed, TState state, Func<IUserMessage, HatarakuCommandContext, TState, Task> continueWith)
            => InternalOk(null, embed, state, continueWith);

        protected HatarakuCommandResult Ok<TState>(string message, LocalEmbed embed, TState state, Func<IUserMessage, HatarakuCommandContext, TState, Task> continueWith)
            => InternalOk(message, embed, state, continueWith);

        protected HatarakuCommandResult Fail(string? reason = null)
            => new HatarakuFailedResult(reason);

        private HatarakuCommandResult InternalOk(string? message, LocalEmbed? embed)
            => new HatarakuReplyResult(message, embed);

        private HatarakuCommandResult InternalOk(string? message, LocalEmbed? embed, Func<IUserMessage, HatarakuCommandContext, Task> continueWith)
            => new HatarakuContinuedExecutionResult(message, embed, continueWith, this.Context);

        private HatarakuCommandResult InternalOk<TState>(string? message, LocalEmbed? embed, TState state, Func<IUserMessage, HatarakuCommandContext, TState, Task> continueWith)
            => new HatarakuContinuedExecutionResult<TState>(message, embed, state, continueWith, this.Context);
    }
}
