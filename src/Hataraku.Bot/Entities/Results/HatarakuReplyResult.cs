using Disqord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hataraku.Bot.Entities.Results
{
    public class HatarakuReplyResult : HatarakuSuccessResult
    {
        public string? Message { get; set; }
        public LocalEmbed? Embed { get; set; }

        public HatarakuReplyResult(string message)
            : this(message, null)
        { }

        public HatarakuReplyResult(LocalEmbed embed)
            : this(null, embed)
        { }

        public HatarakuReplyResult(string? message, LocalEmbed? embed)
        {
            this.Message = message;
            this.Embed = embed;
        }
    }
}
