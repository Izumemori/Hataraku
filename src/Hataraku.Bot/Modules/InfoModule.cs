using Hataraku.Bot.Entities.Commands;
using Qmmands;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Hataraku.Bot.Modules
{
    [Name("Information")]
    [Description("Informative commands about the bot")]
    public partial class InfoModule : HatarakuModuleBase
    {
        [Command("ping")]
        [Description("Returns the Gateway Latency and Round-trip-time")]
        public Task<CommandResult> PingAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            var latencyText = Context.Client.Latency.HasValue ? $"Gateway: {Context.Client.Latency.Value.TotalMilliseconds}ms" : "Gateway: unknown";

            return Ok(latencyText, stopwatch, async (message, context, sw) =>
            {
                sw.Stop();
                await message.ModifyAsync(x => x.Content = message.Content + $"\nRTT: {sw.ElapsedMilliseconds}ms");
            });
        }
    }
}
