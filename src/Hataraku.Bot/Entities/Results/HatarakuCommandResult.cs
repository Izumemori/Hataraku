using Qmmands;
using System.Threading.Tasks;

namespace Hataraku.Bot.Entities.Results
{
    public abstract class HatarakuCommandResult : CommandResult
    {
        public override bool IsSuccessful { get; }

        public HatarakuCommandResult(bool success)
            => this.IsSuccessful = success;

        public static implicit operator Task<CommandResult>(HatarakuCommandResult result)
            => Task.FromResult<CommandResult>(result);
    }
}
