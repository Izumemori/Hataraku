using Disqord;
using Hataraku.Bot.Entities.Commands.Interactivity;

namespace Hataraku.Bot.Entities.Results
{
    public class HatarakuReplyResult : HatarakuSuccessResult
    {
        public string? Message { get; set; }
        public LocalEmbed? Embed { get; set; }
        public InteractiveMessageBuilder? InteractiveMessageBuilder;

        public HatarakuReplyResult(string message)
            : this(message, null, null)
        { }

        public HatarakuReplyResult(LocalEmbed embed)
            : this(null, embed, null)
        { }

        public HatarakuReplyResult(string message, LocalEmbed embed)
            : this(message, embed, null)
        { }

        public HatarakuReplyResult(InteractiveMessageBuilder interactiveMessage)
            : this(null, null, interactiveMessage)
        { }

        public HatarakuReplyResult(string message, InteractiveMessageBuilder interactiveMessage)
            : this(message, null, interactiveMessage)
        { }

        public HatarakuReplyResult(string? message, LocalEmbed? embed, InteractiveMessageBuilder? interactiveMessage)
        {
            this.Message = message;
            this.Embed = embed;
            this.InteractiveMessageBuilder = interactiveMessage;
        }
    }
}
