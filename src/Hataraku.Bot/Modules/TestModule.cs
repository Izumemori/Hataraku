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
            var pages = new[]
            {
                new LocalEmbedBuilder().WithDescription("a"),
                new LocalEmbedBuilder().WithDescription("b"),
                new LocalEmbedBuilder().WithDescription("c"),
                new LocalEmbedBuilder().WithDescription("d"),
                new LocalEmbedBuilder().WithDescription("e"),
            };

            return Ok<PaginatedMessageBuilder>("React to this", x => x.WithPages(pages), (msg, ctx) => 
                msg.ModifyAsync(x =>
                {
                    x.Content = "Bye";
                    x.Embed = null;
                }));
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