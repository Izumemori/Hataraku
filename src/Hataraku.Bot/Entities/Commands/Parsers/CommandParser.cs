using Hataraku.Bot.Entities.Commands.Attributes;
using Hataraku.Bot.Entities.Extensions;
using Qmmands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hataraku.Bot.Entities.Commands.Parsers
{
    public class CommandParser : TypeParser<Command>
    {
        public override ValueTask<TypeParserResult<Command>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            var commands = parameter.Service.GetAllModules()
                .Where(x => CommandUtilities.EnumerateAllCommands(x).Any() && !x.Attributes.Any(x => x is HiddenAttribute) && x.Parent == null)
                .SelectMany(x => CommandUtilities.EnumerateAllCommands(x));

            var startingWith = commands.Where(x => x.FullAliases.Any(y => y.StartsWith(value, StringComparison.OrdinalIgnoreCase)));

            if (startingWith.Any() && !startingWith.Skip(1).Any())
                return TypeParserResult<Command>.Successful(startingWith.First());

            var (command, distance) = commands.Select(x => (x, Distance: x.FullAliases.Select(y => y.ToLower().GetLevenshteinDistance(value.ToLower())).OrderBy(x => x).First()))
                .OrderBy(x => x.Distance)
                .First();

            if (distance > (value.Length * 0.5m))
                return TypeParserResult<Command>.Unsuccessful("Could not find a command by that name");

            return TypeParserResult<Command>.Successful(command);
        }
    }
}
