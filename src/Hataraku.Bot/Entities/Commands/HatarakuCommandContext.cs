using Disqord;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hataraku.Bot.Entities.Commands
{
    public class HatarakuCommandContext : CommandContext
    {
        public IServiceProvider Services { get; set; } 

        public DiscordClient Client { get; set; }

        public string Prefix { get; set; }

        public CachedUserMessage Message { get; set; }

        public ICachedMessageChannel Channel => this.Message.Channel;

        public CachedUser User => this.Message.Author;

        public CachedMember? Member => this.Message.Author as CachedMember;

        public CachedGuild? Guild => (this.Channel as CachedGuildChannel)?.Guild;

        public HatarakuCommandContext(IServiceProvider serviceProvider, string prefix, CachedUserMessage message) : base(serviceProvider)
        {
            this.Services = serviceProvider;
            this.Client = serviceProvider.GetRequiredService<DiscordClient>();
            this.Prefix = prefix;
            this.Message = message;
        }
    }
}
