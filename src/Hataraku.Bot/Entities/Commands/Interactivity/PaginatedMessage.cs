using Disqord;
using Disqord.Events;
using Qommon.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hataraku.Bot.Entities.Commands.Interactivity
{
    public class PaginatedMessage<T> : InteractiveMessage<T>
        where T : DiscordEventArgs
    {
        private readonly IEnumerable<Action<T, PaginatedMessageProperties>> _actions;
        private readonly IEnumerable<IEmoji> _emotes;
        private readonly LocalEmbedBuilder[] _pages;
        private readonly Action<PaginatedMessageState, LocalEmbedFooterBuilder> _footerBuilder;

        private int currentPage;

        public PaginatedMessage(IUserMessage message, Predicate<T> precondition, IEnumerable<Action<T, PaginatedMessageProperties>> actions, IEnumerable<IEmoji> emotes, IEnumerable<LocalEmbedBuilder> pages, Action<PaginatedMessageState, LocalEmbedFooterBuilder> footerBuilder)
            : base(message, precondition)
        {
            this._actions = actions;
            this._emotes = emotes;
            this._pages = pages.ToArray();
            this._footerBuilder = footerBuilder;
        }

        public override async ValueTask SetupAsync()
        {            
            await ChangePageAsync(0);

            foreach (var emote in this._emotes)
                await message.AddReactionAsync(emote);
        }

        public override async ValueTask HandleEventArgsAsync(T args)
        {
            var properties = new PaginatedMessageProperties();

            foreach (var action in this._actions)
                action(args, properties);

            if (args is ReactionAddedEventArgs reactionAddedEventArgs)
                _ = this.message.RemoveMemberReactionAsync(reactionAddedEventArgs.User.Id, reactionAddedEventArgs.Emoji);

            if (properties.ShouldExit.HasValue && properties.ShouldExit.Value)
            {
                this.TaskCompletionSource.SetResult(args);
                return;
            }

            if (properties.Direction.HasValue)
                await ChangePageAsync(properties.Direction.Value);

            if (properties.Page.HasValue)
                await ChangePageAsync(properties.Page.Value);
        }

        private Task ChangePageAsync(PaginatorDirection direction)
            => direction == PaginatorDirection.Forwards
                ? ChangePageAsync(this.currentPage + 1)
                : direction == PaginatorDirection.Backwards
                    ? ChangePageAsync(this.currentPage - 1)
                    : direction == PaginatorDirection.Beginning
                        ? ChangePageAsync(0)
                        : direction == PaginatorDirection.End
                            ? ChangePageAsync(this._pages.Length - 1)
                            : throw new ArgumentException($"Invalid value provided for {nameof(direction)}");

        private Task ChangePageAsync(int page)
        {
            this.currentPage = page >= this._pages.Length
                ? this._pages.Length -1
                : page < 0
                    ? 0
                    : page;

            var embedFooter = new LocalEmbedFooterBuilder();
            this._footerBuilder(new PaginatedMessageState(this.currentPage + 1, this._pages.Length), embedFooter);

            return this.message.ModifyAsync(x => x.Embed = this._pages[this.currentPage].WithFooter(embedFooter).Build());
        }

        public override async ValueTask DisposeAsync()
        {
            try
            {
                await this.message.ClearReactionsAsync();
            }
            catch { }
        }
    }

    public class PaginatedMessageProperties
    {
        public int? Page { get; set; }

        public PaginatorDirection? Direction { get; set; }

        public bool? ShouldExit { get; set; }
    }

    public readonly struct PaginatedMessageState
    {
        public int CurrentPage { get; }
        public int TotalPages { get; }

        public PaginatedMessageState(int currentPage, int totalPages)
        {
            this.CurrentPage = currentPage;
            this.TotalPages = totalPages;
        }
    }
}
