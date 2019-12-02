using Hataraku.Bot.Entities.Commands;
using Hataraku.Bot.Entities.Results;
using Qmmands;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Hataraku.Bot.Modules
{
    public class InfoModule : HatarakuModuleBase
    {
        [Command("ping")]
        public Task<CommandResult> PingAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            return Task.FromResult<CommandResult>(new HatarakuContinuedExecutionResult<Stopwatch>($"Gateway: {Context.Client.Latency}", stopwatch, async (message, sw) =>
            {
                sw.Stop();
                await message.ModifyAsync(x => x.Content = message.Content + $"\nRTT: {sw.ElapsedMilliseconds}ms");
            }));
        }
    }
}
