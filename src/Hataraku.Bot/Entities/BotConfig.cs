#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
using System.Collections.Generic;
using System.Linq;

namespace Hataraku.Bot.Entities
{
    public class BotConfig
    {
        public string Token { get; set; }

        public string Prefixes { get; set; }

        public IEnumerable<string> PrefixEnumerable => this.Prefixes.Split(',').Select(x => x.Trim()); 

        public bool EnableMention { get; set; }
        
        public bool EnableExecutionReaction { get; set; }

        public string SuccessEmoji { get; set; }

        public string FailureEmoji { get; set; }
    }
}
