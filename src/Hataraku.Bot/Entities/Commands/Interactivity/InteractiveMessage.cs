using Disqord;
using Disqord.Events;
using Qommon.Events;
using System;
using System.Threading.Tasks;

namespace Hataraku.Bot.Entities.Commands.Interactivity
{
    public class InteractiveMessage<T> : IInteractiveMessage<T>
        where T : DiscordEventArgs
    {
        protected IUserMessage message;

        public Predicate<T> Precondition { get; }
        public TaskCompletionSource<T> TaskCompletionSource { get; }

        public InteractiveMessage(IUserMessage message, Predicate<T> precondition)
        {
            this.Precondition = precondition;
            this.message = message;
            this.TaskCompletionSource = new TaskCompletionSource<T>();
        }

        public virtual ValueTask SetupAsync()
            => new ValueTask();

        public virtual ValueTask HandleEventArgsAsync(T args)
        {
            this.TaskCompletionSource.SetResult(args);
            return new ValueTask();
        }

        public virtual ValueTask DisposeAsync()
            => new ValueTask();
    }
}
