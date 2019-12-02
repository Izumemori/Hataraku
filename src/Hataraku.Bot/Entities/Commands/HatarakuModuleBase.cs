using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Qmmands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hataraku.Bot.Entities.Commands
{
    public abstract class HatarakuModuleBase : HatarakuModuleBase<HatarakuCommandContext>
    { }

    public class HatarakuModuleBase<T> : ModuleBase<T>
        where T: HatarakuCommandContext
    {
        private ILogger<HatarakuModuleBase<T>> logger => Context.ServiceProvider.GetRequiredService<ILogger<HatarakuModuleBase<T>>>();

        protected override ValueTask BeforeExecutedAsync()
        {
            this.logger.LogDebug($"Executing {Context.Command} by {Context.User} in {FormatChannel()}");

            return default;
        }

        protected override ValueTask AfterExecutedAsync()
        {
            this.logger.LogDebug($"Executed {Context.Command} by {Context.User} in {FormatChannel()}");

            return default;
        }

        private string FormatChannel()
            => Context.Guild != null
                ? $"{Context.Channel} ({Context.Channel.Id}); Guild: {Context.Guild} ({Context.Guild.Id})"
                : $"{Context.Channel} ({Context.Channel.Id})";
    }
}
