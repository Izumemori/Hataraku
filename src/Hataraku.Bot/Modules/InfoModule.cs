using Hataraku.Bot.Entities.Commands;
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
            var latencyText = Context.Client.Latency.HasValue ? $"Gateway: {Context.Client.Latency.Value.TotalMilliseconds}ms" : "Gateway: unknown";

            return Ok(latencyText, stopwatch, async (message, sw) =>
            {
                sw.Stop();
                await message.ModifyAsync(x => x.Content = message.Content + $"\nRTT: {sw.ElapsedMilliseconds}ms");
            });
        }
    }
}
