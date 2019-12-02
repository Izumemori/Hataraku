using Disqord;
using System;
using System.Threading.Tasks;

namespace Hataraku.Bot.Entities.Results
{
    public class HatarakuContinuedExecutionResult : HatarakuReplyResult
    {
        public Func<IUserMessage, Task> ContinueWith { get; }

        public HatarakuContinuedExecutionResult(string message, Func<IUserMessage, Task> continueWith)
            : base(message)
            => this.ContinueWith = continueWith;

        public HatarakuContinuedExecutionResult(LocalEmbed embed, Func<IUserMessage, Task> continueWith)
            : base(embed)
            => this.ContinueWith = continueWith;

        public HatarakuContinuedExecutionResult(string? message, LocalEmbed? embed, Func<IUserMessage, Task> continueWith)
            : base(message, embed)
            => this.ContinueWith = continueWith;
    }

    public class HatarakuContinuedExecutionResult<T> : HatarakuContinuedExecutionResult
    {
        public HatarakuContinuedExecutionResult(string message, T state, Func<IUserMessage, T, Task> continueWith)
            : base(message, (message) => continueWith(message, state))
        {}

        public HatarakuContinuedExecutionResult(LocalEmbed embed, T state, Func<IUserMessage, T, Task> continueWith)
            : base(embed, (message) => continueWith(message, state))
        {}

        public HatarakuContinuedExecutionResult(string? message, LocalEmbed? embed, T state, Func<IUserMessage, T, Task> continueWith)
            : base(message, embed, (message) => continueWith(message, state))
        {}
    }
}
