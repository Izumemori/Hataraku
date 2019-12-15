#if DEBUG
using Disqord;
using Disqord.Events;
using Hataraku.Bot.Entities.Commands;
using Hataraku.Bot.Entities.Commands.Interactivity;
using Hataraku.Bot.Entities.Results;
using Hataraku.Bot.Services;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using System;
using System.Threading.Tasks;

namespace Hataraku.Bot.Modules
{
    [Name("Tests")]
    [Description("Various owner only tests")]
    public class TestModule : HatarakuModuleBase
    {
        [Command("test")]
        public Task<CommandResult> TestAsync()
        {
            return Ok("React to this", async (message, context) =>
            {
                var interactiveService = context.ServiceProvider.GetRequiredService<InteractionService>();

                Predicate<DiscordEventArgs> predicate = (x =>
                {
                    return x switch
                    {
                        ReactionAddedEventArgs reactionAdded => reactionAdded.User.Id == context.User.Id && reactionAdded.Message.Id == message.Id,
                        MessageReceivedEventArgs messageReceived => messageReceived.Message.Author.Id == context.User.Id,
                        _ => false
                    };
                });

                var emoji = new[]
                {
                    new LocalEmoji("◀"),
                    new LocalEmoji("▶"),
                    new LocalEmoji("⏹")
                };

                var actions = new Action<DiscordEventArgs, PaginatedMessageProperties>[]
                {
                    (args, props) =>
                    {
                        if (!(args is ReactionAddedEventArgs reactionAdded)) return;
                        if (reactionAdded.Emoji.Name == emoji[0].Name) props.Direction = PaginatorDirection.Backwards;
                        if (reactionAdded.Emoji.Name == emoji[1].Name) props.Direction = PaginatorDirection.Forwards;
                        if (reactionAdded.Emoji.Name == emoji[2].Name) props.ShouldExit = true;
                    },
                    (args, props) =>
                    {
                        if (!(args is MessageReceivedEventArgs messageReceived)) return;
                        if (!int.TryParse(messageReceived.Message.Content, out var res)) return;
                        props.Page = res;
                    }
                };

                var pages = new LocalEmbedBuilder[]
                {
                    new LocalEmbedBuilder().WithDescription("1"),
                    new LocalEmbedBuilder().WithDescription("2"),
                    new LocalEmbedBuilder().WithDescription("3")
                };

                Action<PaginatedMessageState, LocalEmbedFooterBuilder> footer = (x, footer) => footer.WithText($"Page {x.CurrentPage}/{x.TotalPages}");

                var interactiveMessage = new PaginatedMessage<DiscordEventArgs>(message, predicate, actions, emoji, pages, footer);

                var res = await interactiveService.WaitForAsync(interactiveMessage);
            });
        }

        [Command("test2")]
        public Task<CommandResult> Test2Async([Remainder] string? reason = null)
            => Task.FromResult<CommandResult>(new HatarakuFailedResult(reason));

        [Command("test3")]
        public Task<CommandResult> Test3Async()
            => Task.FromResult<CommandResult>(new HatarakuReplyResult("Pong!"));
    }
}
#endif