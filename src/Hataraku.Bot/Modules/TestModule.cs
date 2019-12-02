#if DEBUG
using Hataraku.Bot.Entities.Commands;
using Hataraku.Bot.Entities.Results;
using Qmmands;
using System.Threading.Tasks;

namespace Hataraku.Bot.Modules
{
    public class TestModule : HatarakuModuleBase
    {
        [Command("test")]
        public async Task<CommandResult> TestAsync()
        {
            await Task.Delay(1000);

            return new HatarakuSuccessResult();
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