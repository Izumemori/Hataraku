using Disqord;
using Disqord.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hataraku.Bot.Entities.Commands.Interactivity
{
    public abstract class InteractiveMessageBuilder
    {
        protected readonly HatarakuCommandContext context;
        protected IUserMessage? message;

        public InteractiveMessageBuilder(HatarakuCommandContext context)
            => this.context = context;

        public InteractiveMessageBuilder WithMessage(IUserMessage message)
        {
            this.message = message;
            return this;
        }

        public abstract IInteractiveMessage Build();
    }

    public class InteractiveMessageBuilder<T> : InteractiveMessageBuilder
        where T: DiscordEventArgs
    {
        protected Func<T, HatarakuCommandContext, IUserMessage, bool>? precondition;

        public InteractiveMessageBuilder(HatarakuCommandContext context)
            : base(context)
        {}

        public override IInteractiveMessage Build()
            => new InteractiveMessage<T>(context,
                message ?? throw new ArgumentException($"A {nameof(message)} has to be provided"),
                precondition ?? throw new ArgumentException($"A {nameof(precondition)} has to be provided"));

        public InteractiveMessageBuilder<T> WithPrecondition(Func<T, HatarakuCommandContext, IUserMessage, bool> precondition)
        {
            this.precondition = precondition;

            return this;
        }
    }
}
