using Disqord;
using Disqord.Events;
using Qommon.Events;
using System;
using System.Threading.Tasks;

namespace Hataraku.Bot.Entities.Commands.Interactivity
{
    public interface IInteractiveMessage<T> : IAsyncDisposable
        where T: DiscordEventArgs
    {
        Predicate<T> Precondition { get; }

        TaskCompletionSource<T> TaskCompletionSource { get; }

        ValueTask SetupAsync();

        ValueTask HandleEventArgsAsync(T args);
    }
}
