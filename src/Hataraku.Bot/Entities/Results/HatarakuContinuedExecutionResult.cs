using Disqord;
using Hataraku.Bot.Entities.Commands;
using System;
using System.Threading.Tasks;

namespace Hataraku.Bot.Entities.Results
{
    public class HatarakuContinuedExecutionResult : HatarakuReplyResult
    {
        public Func<IUserMessage, Task> ContinueWith { get; }

        public HatarakuContinuedExecutionResult(string message, Func<IUserMessage, HatarakuCommandContext, Task> continueWith, HatarakuCommandContext context)
            : base(message)
            => this.ContinueWith = (msg) => continueWith(msg, context);

        public HatarakuContinuedExecutionResult(LocalEmbed embed, Func<IUserMessage, HatarakuCommandContext, Task> continueWith, HatarakuCommandContext context)
            : base(embed)
            => this.ContinueWith = (msg) => continueWith(msg, context);

        public HatarakuContinuedExecutionResult(string? message, LocalEmbed? embed, Func<IUserMessage, HatarakuCommandContext, Task> continueWith, HatarakuCommandContext context)
            : base(message, embed)
            => this.ContinueWith = (msg) => continueWith(msg, context);
    }

    public class HatarakuContinuedExecutionResult<T> : HatarakuContinuedExecutionResult
    {
        public HatarakuContinuedExecutionResult(string message, T state, Func<IUserMessage, HatarakuCommandContext, T, Task> continueWith, HatarakuCommandContext context)
            : base(message, (msg, ctx) => continueWith(msg, ctx, state), context)
        {}

        public HatarakuContinuedExecutionResult(LocalEmbed embed, T state, Func<IUserMessage, HatarakuCommandContext, T, Task> continueWith, HatarakuCommandContext context)
            : base(embed, (msg, ctx) => continueWith(msg, ctx, state), context)
        {}

        public HatarakuContinuedExecutionResult(string? message, LocalEmbed? embed, T state, Func<IUserMessage, HatarakuCommandContext, T, Task> continueWith, HatarakuCommandContext context)
            : base(message, embed, (msg, ctx) => continueWith(msg, ctx, state), context)
        {}
    }
}
