using Disqord;
using Hataraku.Bot.Entities.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Qmmands;
using System;
using System.Collections.Generic;
using System.Text;
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

        protected Task<CommandResult> Ok(string? message = null, LocalEmbed? embed = null)
            => Task.FromResult<CommandResult>(new HatarakuReplyResult(message, embed));

        protected Task<CommandResult> Ok(string message, Func<IUserMessage, Task> continueWith)
            => InternalOk(message, null, continueWith);

        protected Task<CommandResult> Ok(LocalEmbed embed, Func<IUserMessage, Task> continueWith)
            => InternalOk(null, embed, continueWith);

        protected Task<CommandResult> Ok(string message, LocalEmbed embed, Func<IUserMessage, Task> continueWith)
            => InternalOk(message, embed, continueWith);

        protected Task<CommandResult> Ok<TState>(string message, TState state, Func<IUserMessage, TState, Task> continueWith)
            => InternalOk(message, null, state, continueWith);

        protected Task<CommandResult> Ok<TState>(LocalEmbed embed, TState state, Func<IUserMessage, TState, Task> continueWith)
            => InternalOk(null, embed, state, continueWith);

        protected Task<CommandResult> Ok<TState>(string message, LocalEmbed embed, TState state, Func<IUserMessage, TState, Task> continueWith)
            => InternalOk(message, embed, state, continueWith);

        protected Task<CommandResult> Fail(string? reason = null)
            => Task.FromResult<CommandResult>(new HatarakuFailedResult(reason));

        private Task<CommandResult> InternalOk(string? message, LocalEmbed? embed, Func<IUserMessage, Task> continueWith)
            => Task.FromResult<CommandResult>(new HatarakuContinuedExecutionResult(message, embed, continueWith));

        private Task<CommandResult> InternalOk<TState>(string? message, LocalEmbed? embed, TState state, Func<IUserMessage, TState, Task> continueWith)
            => Task.FromResult<CommandResult>(new HatarakuContinuedExecutionResult<TState>(message, embed, state, continueWith));
    }
}
