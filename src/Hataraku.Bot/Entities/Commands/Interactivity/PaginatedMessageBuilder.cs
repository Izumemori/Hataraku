using Disqord;
using Disqord.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hataraku.Bot.Entities.Commands.Interactivity
{
    public class PaginatedMessageBuilder : PaginatedMessageBuilder<DiscordEventArgs>
    {
        public PaginatedMessageBuilder(HatarakuCommandContext context) : base(context)
        {}
    }

    public partial class PaginatedMessageBuilder<T> : InteractiveMessageBuilder<T>
        where T: DiscordEventArgs
    {
        private readonly HatarakuCommandContext _context;
        private readonly List<LocalEmbedBuilder> embedBuilders;
        private readonly List<Action<T, PaginatedMessageState, PaginatedMessageProperties>> actions;
        private readonly List<IEmoji> emotes;
        private bool withoutFooter;

        private Action<PaginatedMessageState, LocalEmbedFooterBuilder>? footerBuilder;

        public PaginatedMessageBuilder(HatarakuCommandContext context)
            : base(context)
        {
            this._context = context;
            this.embedBuilders = new List<LocalEmbedBuilder>();
            this.actions = new List<Action<T, PaginatedMessageState, PaginatedMessageProperties>>();
            this.emotes = new List<IEmoji>();
        }

        public new PaginatedMessageBuilder<T> WithMessage(IUserMessage message)
        {
            base.WithMessage(message);

            return this;
        }

        public new PaginatedMessageBuilder<T> WithPrecondition(Func<T, HatarakuCommandContext, IUserMessage, bool> precondition)
        {
            base.WithPrecondition(precondition);

            return this;
        }

        public PaginatedMessageBuilder<T> WithEmote(IEmoji emote)
        {
            this.emotes.Add(emote);

            return this;
        }

        public PaginatedMessageBuilder<T> WithEmotes(IEnumerable<IEmoji> emotes)
        {
            this.emotes.AddRange(emotes);

            return this;
        }

        public PaginatedMessageBuilder<T> WithPage(Action<LocalEmbedBuilder> embedBuilder)
        {
            var builder = new LocalEmbedBuilder();
            embedBuilder(builder);
            this.embedBuilders.Add(builder);

            return this;
        }

        public PaginatedMessageBuilder<T> WithPages(IEnumerable<LocalEmbedBuilder> embedBuilders)
        {
            this.embedBuilders.AddRange(embedBuilders);

            return this;
        }

        public PaginatedMessageBuilder<T> WithAction(Action<T, PaginatedMessageState, PaginatedMessageProperties> action)
        {
            this.actions.Add(action);

            return this;
        }

        public PaginatedMessageBuilder<T> WithFooter(Action<PaginatedMessageState, LocalEmbedFooterBuilder> footerBuilder)
        {
            this.footerBuilder = footerBuilder;

            return this;
        }

        public PaginatedMessageBuilder<T> WithoutDynamicFooter()
        {
            this.withoutFooter = true;

            return this;
        }

        public override IInteractiveMessage Build()
            => new PaginatedMessage<T>(
                this._context,
                this.message ?? throw new ArgumentException($"A {nameof(message)} has to be provided"),
                this.precondition ?? DefaultPredicate,
                this.actions.Count == 0 ? DefaultActions : this.actions.ToArray(),
                this.emotes.Count == 0 ? DefaultEmotes : this.emotes.ToArray(),
                this.embedBuilders,
                this.withoutFooter && this.footerBuilder == null ? null : this.footerBuilder ?? DefaultFooterBuilder
                );
    }
}
