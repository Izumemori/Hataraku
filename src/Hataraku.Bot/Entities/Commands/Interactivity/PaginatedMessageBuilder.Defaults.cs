using Disqord;
using Disqord.Events;
using System;

namespace Hataraku.Bot.Entities.Commands.Interactivity
{
    public partial class PaginatedMessageBuilder<T>
    {
        private const string Stop = "❌";
        private const string End = "⏭";
        private const string Beginning = "⏮";
        private const string Next = "▶";
        private const string Previous = "◀";
        private const string Selector = "🔢";

        private static readonly IEmoji[] DefaultEmotes = new IEmoji[]
        {
            new LocalEmoji(Beginning),
            new LocalEmoji(Previous),
            new LocalEmoji(Next),
            new LocalEmoji(End),
            new LocalEmoji(Selector),
            new LocalEmoji(Stop)
        };

        private static readonly Func<T, HatarakuCommandContext, IUserMessage, bool> DefaultPredicate = (args, context, message) => args switch
        {
            ReactionAddedEventArgs reactionAdded => reactionAdded.User.Id == context.User.Id && reactionAdded.Message.Id == message.Id,
            MessageReceivedEventArgs messageReceived => messageReceived.Message.Author.Id == context.User.Id && int.TryParse(messageReceived.Message.Content, out _),
            _ => false
        };

        private static readonly Action<T, PaginatedMessageState, PaginatedMessageProperties>[] DefaultActions = new Action<T, PaginatedMessageState, PaginatedMessageProperties>[]
        {
            (args, state, props) =>
            {
                if (!(args is ReactionAddedEventArgs reactionAdded)) return;
                switch (reactionAdded.Emoji.Name)
                {
                    case Next:
                        props.Direction = PaginatorDirection.Forwards;
                        break;
                    case Previous:
                        props.Direction = PaginatorDirection.Backwards;
                        break;
                    case Beginning:
                        props.Direction = PaginatorDirection.Beginning;
                        break;
                    case End:
                        props.Direction = PaginatorDirection.End;
                        break;
                    case Stop:
                        props.ShouldExit = true;
                        break;
                    case Selector:
                        props.ShouldWaitForMessage = true;
                        break;
                }
            },
            (args, state, props) =>
            {
                if (!state.IsAwaitingMessage) return;

                if (!(args is MessageReceivedEventArgs messageReceived)) return;
                if (!int.TryParse(messageReceived.Message.Content, out var result) || result < 1 || result > state.TotalPages) return;

                props.Page = result - 1;
                props.ShouldWaitForMessage = false;
            }
        };

        private static readonly Action<PaginatedMessageState, LocalEmbedFooterBuilder> DefaultFooterBuilder = (state, builder) =>
        {
            if (state.IsAwaitingMessage)
            {
                builder.WithText($"Please reply with the page number you want to switch to. [1-{state.TotalPages}]");
                return;
            }

            builder.WithText($"Page {state.CurrentPage}/{state.TotalPages}");
        };
    }
}
