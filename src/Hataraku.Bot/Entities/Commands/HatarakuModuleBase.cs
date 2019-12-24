using Disqord;
using Hataraku.Bot.Entities.Commands.Interactivity;
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
            => InternalOk(message, null, null);

        protected HatarakuCommandResult Ok(LocalEmbed embed)
            => InternalOk(null, embed, null);

        protected HatarakuCommandResult Ok(string message, LocalEmbed embed)
            => InternalOk(message, embed, null);

        protected HatarakuCommandResult Ok(string message, InteractiveMessageBuilder builder)
            => InternalOk(message, null, builder);
        
        protected HatarakuCommandResult OK<TBuilder>(TBuilder builder, Action<TBuilder>? messageBuilder = null)
            where TBuilder: InteractiveMessageBuilder
            => InternalOk<TBuilder>(null, builder, messageBuilder);

        protected HatarakuCommandResult Ok<TBuilder>(string message, Action<TBuilder>? messageBuilder = null)
            where TBuilder: InteractiveMessageBuilder
            => InternalOk<TBuilder>(message, null, messageBuilder);

        protected HatarakuCommandResult Ok<TBuilder>(string message, TBuilder builder, Action<TBuilder>? messageBuilder = null)
            where TBuilder: InteractiveMessageBuilder
            => InternalOk<TBuilder>(message, builder, messageBuilder);

        protected HatarakuCommandResult Ok(string message, Func<IUserMessage, HatarakuCommandContext, Task> continueWith)
            => InternalOk(message, null, null, continueWith);

        protected HatarakuCommandResult Ok(LocalEmbed embed, Func<IUserMessage, HatarakuCommandContext, Task> continueWith)
            => InternalOk(null, embed, null, continueWith);

        protected HatarakuCommandResult Ok(string message, InteractiveMessageBuilder builder, Func<IUserMessage, HatarakuCommandContext, Task> continueWith)
            => InternalOk(message, null, builder, continueWith);

        protected HatarakuCommandResult Ok<TBuilder>(Action<TBuilder> messageBuilder, Func<IUserMessage, HatarakuCommandContext, Task> continueWith)
            where TBuilder: InteractiveMessageBuilder
            => InternalOk<TBuilder>(null, null, messageBuilder, continueWith);

        protected HatarakuCommandResult Ok<TBuilder>(string message, TBuilder builder, Func<IUserMessage, HatarakuCommandContext, Task> continueWith)
            where TBuilder: InteractiveMessageBuilder
            => InternalOk<TBuilder>(message, builder, null, continueWith);

        protected HatarakuCommandResult Ok<TBuilder>(string message, Action<TBuilder> messageBuilder, Func<IUserMessage, HatarakuCommandContext, Task> continueWith)
            where TBuilder: InteractiveMessageBuilder
            => InternalOk<TBuilder>(message, null, messageBuilder, continueWith);

        protected HatarakuCommandResult Ok<TBuilder>(string message, TBuilder builder, Action<TBuilder> messageBuilder, Func<IUserMessage, HatarakuCommandContext, Task> continueWith)
            where TBuilder: InteractiveMessageBuilder
            => InternalOk<TBuilder>(message, builder, messageBuilder, continueWith);

        protected HatarakuCommandResult Ok<TBuilder>(TBuilder builder, Func<IUserMessage, HatarakuCommandContext, Task> continueWith)
            where TBuilder: InteractiveMessageBuilder
            => InternalOk<TBuilder>(null, builder, null, continueWith);

        protected HatarakuCommandResult Ok<TBuilder>(TBuilder builder, Action<TBuilder> messageBuilder, Func<IUserMessage, HatarakuCommandContext, Task> continueWith)
            where TBuilder: InteractiveMessageBuilder
            => InternalOk<TBuilder>(null, builder, messageBuilder, continueWith);

        protected HatarakuCommandResult Ok(string message, LocalEmbed embed, Func<IUserMessage, HatarakuCommandContext, Task> continueWith)
            => InternalOk(message, embed, null, continueWith);

        protected HatarakuCommandResult Ok<TState>(string message, TState state, Func<IUserMessage, HatarakuCommandContext, TState, Task> continueWith)
            => InternalOk(message, null, null, state, continueWith);

        protected HatarakuCommandResult Ok<TState>(LocalEmbed embed, TState state, Func<IUserMessage, HatarakuCommandContext, TState, Task> continueWith)
            => InternalOk(null, embed, null, state, continueWith);

        protected HatarakuCommandResult Ok<TState>(string message, LocalEmbed embed, TState state, Func<IUserMessage, HatarakuCommandContext, TState, Task> continueWith)
            => InternalOk(message, embed, null, state, continueWith);

        protected HatarakuCommandResult Ok<TState, TBuilder>(string message, TBuilder builder, TState state, Func<IUserMessage, HatarakuCommandContext, TState, Task> continueWith)
            where TBuilder: InteractiveMessageBuilder
            => InternalOk<TState, TBuilder>(message, builder, null, state, continueWith);

        protected HatarakuCommandResult Ok<TState, TBuilder>(string message, TBuilder builder, Action<TBuilder> messageBuilder, TState state, Func<IUserMessage, HatarakuCommandContext, TState, Task> continueWith)
            where TBuilder: InteractiveMessageBuilder
            => InternalOk<TState, TBuilder>(message, builder, messageBuilder, state, continueWith);

        protected HatarakuCommandResult Ok<TState, TBuilder>(Action<TBuilder> messageBuilder, TState state, Func<IUserMessage, HatarakuCommandContext, TState, Task> continueWith)
            where TBuilder: InteractiveMessageBuilder
            => InternalOk<TState, TBuilder>(null, null, messageBuilder, state, continueWith);

        protected HatarakuCommandResult Ok<TState, TBuilder>(TBuilder builder, TState state, Func<IUserMessage, HatarakuCommandContext, TState, Task> continueWith)
            where TBuilder: InteractiveMessageBuilder
            => InternalOk<TState, TBuilder>(null, builder, null, state, continueWith);

        protected HatarakuCommandResult Ok<TState, TBuilder>(TBuilder builder, Action<TBuilder> messageBuilder, TState state, Func<IUserMessage, HatarakuCommandContext, TState, Task> continueWith)
            where TBuilder: InteractiveMessageBuilder
            => InternalOk<TState, TBuilder>(null, builder, messageBuilder, state, continueWith);

        protected HatarakuCommandResult Fail(string? reason = null)
            => new HatarakuFailedResult(reason);

        private HatarakuCommandResult InternalOk(string? message, LocalEmbed? embed, InteractiveMessageBuilder? interactiveMessageBuilder)
            => new HatarakuReplyResult(message, embed, interactiveMessageBuilder);

        private HatarakuCommandResult InternalOk<TBuilder>(string? message, TBuilder? interactiveMessageBuilder, Action<TBuilder>? messageBuilder)
            where TBuilder : InteractiveMessageBuilder
        {
            var msgBuilder = interactiveMessageBuilder ?? TypeUtils.CreateInstance<TBuilder>(this.Context);
            messageBuilder?.Invoke(msgBuilder);
            return new HatarakuReplyResult(message, null, msgBuilder);
        }

        private HatarakuCommandResult InternalOk(string? message, LocalEmbed? embed, InteractiveMessageBuilder? interactiveMessageBuilder, Func<IUserMessage, HatarakuCommandContext, Task> continueWith)
            => new HatarakuContinuedExecutionResult(message, embed, interactiveMessageBuilder, continueWith, this.Context);

        private HatarakuCommandResult InternalOk<TBuilder>(string? message, TBuilder? interactiveMessageBuilder, Action<TBuilder>? messageBuilder, Func<IUserMessage, HatarakuCommandContext, Task> continueWith)
            where TBuilder: InteractiveMessageBuilder
        {
            var msgBuilder = interactiveMessageBuilder ?? TypeUtils.CreateInstance<TBuilder>(Context);
            messageBuilder?.Invoke(msgBuilder);
            return new HatarakuContinuedExecutionResult(message, null, msgBuilder, continueWith, this.Context);
        }

        private HatarakuCommandResult InternalOk<TState>(string? message, LocalEmbed? embed, InteractiveMessageBuilder? interactiveMessageBuilder, TState state, Func<IUserMessage, HatarakuCommandContext, TState, Task> continueWith)
            => new HatarakuContinuedExecutionResult<TState>(message, embed, interactiveMessageBuilder, state, continueWith, this.Context);

        private HatarakuCommandResult InternalOk<TState, TBuilder>(string? message, TBuilder? interactiveMessageBuilder, Action<TBuilder>? messageBuilder, TState state, Func<IUserMessage, HatarakuCommandContext, TState, Task> continueWith)
            where TBuilder: InteractiveMessageBuilder
        {
            var msgBuilder = interactiveMessageBuilder ?? TypeUtils.CreateInstance<TBuilder>(Context);
            messageBuilder?.Invoke(msgBuilder);
            return new HatarakuContinuedExecutionResult<TState>(message, null, msgBuilder, state, continueWith, this.Context);
        }
    }
}
