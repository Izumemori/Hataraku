namespace Hataraku.Bot.Entities.Results
{
    public class HatarakuFailedResult : HatarakuCommandResult
    {
        public string? Reason { get; }

        public HatarakuFailedResult(string? reason)
            : base(false)
            => this.Reason = reason;
    }
}
