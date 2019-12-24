using Disqord;
using Disqord.Events;
using System;
using System.Threading.Tasks;

namespace Hataraku.Bot.Entities.Commands.Interactivity
{
    public interface IInteractiveMessage: IAsyncDisposable
    {
        ValueTask SetupAsync();
        Predicate<DiscordEventArgs> Precondition { get; }
        TaskCompletionSource<IUserMessage> TaskCompletionSource { get; }
        ValueTask<bool> HandleEventArgsAsync(DiscordEventArgs args);
        
    }
}
