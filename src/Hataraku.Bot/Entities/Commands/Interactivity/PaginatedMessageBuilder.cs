using Disqord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hataraku.Bot.Entities.Commands.Interactivity
{
    public class PaginatedMessageBuilder
    {
        private const string Stop = "❌";

        private const string End = "⏭";

        private const string Beginning = "⏮";

        private const string Next = "▶";

        private const string Previous = "◀";

        private const string Selector = "🔢";

        private readonly IServiceProvider _serviceProvider;
        private List<LocalEmbedBuilder> embedBuilders;

        public PaginatedMessageBuilder(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
            this.embedBuilders = new List<LocalEmbedBuilder>();
        }

        public PaginatedMessageBuilder WithPage(Action<LocalEmbedBuilder> embedBuilder)
        {
            var builder = new LocalEmbedBuilder();
            embedBuilder(builder);

            if (builder.Footer != null) throw new ArgumentException($"{nameof(builder.Footer)} should not be set.");

            this.embedBuilders.Add(builder);

            return this;
        }
    }
}
