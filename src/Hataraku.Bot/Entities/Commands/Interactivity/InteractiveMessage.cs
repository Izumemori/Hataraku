using Disqord;
using Disqord.Events;
using Qommon.Events;
using System;
using System.Threading.Tasks;

namespace Hataraku.Bot.Entities.Commands.Interactivity
{
    public class InteractiveMessage<T> : IInteractiveMessage
        where T: DiscordEventArgs
    {
        protected IUserMessage message;
        protected HatarakuCommandContext context;

        public Predicate<T> Precondition { get; }
        public TaskCompletionSource<IUserMessage> TaskCompletionSource { get; }

        Predicate<DiscordEventArgs> IInteractiveMessage.Precondition => (x) => Precondition((T)x);

        public InteractiveMessage(HatarakuCommandContext context, IUserMessage message, Func<T, HatarakuCommandContext, IUserMessage, bool> precondition)
        {
            this.Precondition = (args) => precondition(args, this.context, this.message);
            this.context = context;
            this.message = message;
            this.TaskCompletionSource = new TaskCompletionSource<IUserMessage>();
        }

        public virtual ValueTask SetupAsync()
            => new ValueTask();

        public virtual ValueTask<bool> HandleEventArgsAsync(T args)
        {
            this.TaskCompletionSource.SetResult(this.message);
            return new ValueTask<bool>(true);
        }

        public virtual ValueTask DisposeAsync()
            => new ValueTask();

        ValueTask<bool> IInteractiveMessage.HandleEventArgsAsync(DiscordEventArgs args)
        {
            if (!(args is T eventArgs)) return new ValueTask<bool>(false);

            return HandleEventArgsAsync(eventArgs);
        }
    }
}
