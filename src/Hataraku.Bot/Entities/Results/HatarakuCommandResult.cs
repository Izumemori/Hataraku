using Qmmands;

namespace Hataraku.Bot.Entities.Results
{
    public abstract class HatarakuCommandResult : CommandResult
    {
        public override bool IsSuccessful { get; }

        public HatarakuCommandResult(bool success)
            => this.IsSuccessful = success;
    }
}
