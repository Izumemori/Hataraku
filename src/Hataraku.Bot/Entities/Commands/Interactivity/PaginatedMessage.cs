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
        private readonly IEnumerable<Action<T, PaginatedMessageState, PaginatedMessageProperties>> _actions;
        private readonly IEnumerable<IEmoji> _emotes;
        private readonly LocalEmbedBuilder[] _pages;
        private readonly Action<PaginatedMessageState, LocalEmbedFooterBuilder>? _footerBuilder;

        private int currentPage;
        private bool waitingForMessage;
        private PaginatedMessageState state => new PaginatedMessageState(this.currentPage + 1, this._pages.Length, waitingForMessage);

        public PaginatedMessage(HatarakuCommandContext context, IUserMessage message, Func<T, HatarakuCommandContext, IUserMessage, bool> precondition, IEnumerable<Action<T, PaginatedMessageState, PaginatedMessageProperties>> actions, IEnumerable<IEmoji> emotes, IEnumerable<LocalEmbedBuilder> pages, Action<PaginatedMessageState, LocalEmbedFooterBuilder>? footerBuilder = null)
            : base(context, message, precondition)
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

        public override async ValueTask<bool> HandleEventArgsAsync(T args)
        {
            var properties = new PaginatedMessageProperties();

            foreach (var action in this._actions)
                action(args, this.state, properties);

            if (args is ReactionAddedEventArgs reactionAddedEventArgs)
                _ = this.message.RemoveMemberReactionAsync(reactionAddedEventArgs.User.Id, reactionAddedEventArgs.Emoji);

            if (properties.ShouldExit.HasValue && properties.ShouldExit.Value)
            {
                this.TaskCompletionSource.SetResult(this.message);
                return true;
            }

            if (properties.Direction.HasValue)
                await ChangePageAsync(properties.Direction.Value);

            if (properties.Page.HasValue)
                await ChangePageAsync(properties.Page.Value);

            if (properties.ShouldWaitForMessage.HasValue)
            {
                this.waitingForMessage = properties.ShouldWaitForMessage.Value;
                await ChangePageAsync(this.currentPage);
            }

            return true;
        }

        private Task ChangePageAsync(PaginatorDirection direction)
            => direction switch
            {
                PaginatorDirection.Forwards => ChangePageAsync(this.currentPage + 1),
                PaginatorDirection.Backwards => ChangePageAsync(this.currentPage - 1),
                PaginatorDirection.Beginning => ChangePageAsync(0),
                PaginatorDirection.End => ChangePageAsync(this._pages.Length - 1),
                _ => throw new ArgumentException($"Invalid value provided for {nameof(direction)}")
            };

        private Task ChangePageAsync(int page)
        {
            this.currentPage = page >= this._pages.Length
                ? this._pages.Length - 1
                : page < 0
                    ? 0
                    : page;

            var embedFooter = new LocalEmbedFooterBuilder();

            if (this._footerBuilder != null)
            {
                this._footerBuilder(this.state, embedFooter);

                return this.message.ModifyAsync(x => x.Embed = this._pages[this.currentPage].WithFooter(embedFooter).Build());
            }

            return this.message.ModifyAsync(x => x.Embed = this._pages[this.currentPage].Build());
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

        public bool? ShouldWaitForMessage { get; set; }
    }

    public readonly struct PaginatedMessageState
    {
        public int CurrentPage { get; }
        public int TotalPages { get; }
        public bool IsAwaitingMessage { get; }

        public PaginatedMessageState(int currentPage, int totalPages, bool isAwaitingMessage)
        {
            this.CurrentPage = currentPage;
            this.TotalPages = totalPages;
            this.IsAwaitingMessage = isAwaitingMessage;
        }
    }
}
