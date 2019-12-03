using Hataraku.Bot.Entities.Commands.Attributes;
using Hataraku.Bot.Entities.Extensions;
using Qmmands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hataraku.Bot.Entities.Commands.Parsers
{
    public class ModuleParser : TypeParser<Module>
    {
        public override ValueTask<TypeParserResult<Module>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            var modules = parameter.Service.GetAllModules()
                .Where(x => CommandUtilities.EnumerateAllCommands(x).Any() && !x.Attributes.Any(x => x is HiddenAttribute) && x.Parent == null);

            var startingWith = modules.Where(x => x.Name.StartsWith(value, StringComparison.OrdinalIgnoreCase));

            if (startingWith.Any() && !startingWith.Skip(1).Any())
                return TypeParserResult<Module>.Successful(startingWith.First());

            (int distance, Module module) = modules.Select(x => (Distance: x.Name.ToLower().GetLevenshteinDistance(value.ToLower()), x))
                .OrderBy(x => x.Distance)
                .First();

            if (distance > (value.Length * 0.5m))
                return TypeParserResult<Module>.Unsuccessful("Couldn't find a module by that name");

            return TypeParserResult<Module>.Successful(module);
        }
    }
}