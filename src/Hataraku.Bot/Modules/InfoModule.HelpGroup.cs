using Disqord;
using Hataraku.Bot.Entities.Commands;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hataraku.Bot.Modules
{
    public partial class InfoModule
    {
        [Group("help")]
        [Description("Query categories and commands")]
        public class HelpGroup : HatarakuModuleBase
        {
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
            private CommandService commandService;
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

            protected override ValueTask BeforeExecutedAsync()
            {
                this.commandService = commandService = this.Context.ServiceProvider.GetRequiredService<CommandService>();

                return base.BeforeExecutedAsync();
            }

            [Command]
            public Task<CommandResult> HelpAsync()
            {
                var modules = this.commandService.GetAllModules();

                var embed = new LocalEmbedBuilder()
                    .WithTitle("Help")
                    .WithDescription("The following categories are available")
                    .WithFooter($"To list commands use {this.Context.Prefix}help <category name>");

                foreach (var module in modules)
                {
                    if (module.Parent != null) continue;

                    embed.AddField(module.Name, module.Description ?? "Unknown", false);
                }

                return Ok(embed.Build());
            }

            [Command("", "module")]
            [Priority(1)]
            public Task<CommandResult> HelpAsync([Remainder, Description("The name of the category you want help for"), Name("category")] Module module)
            {
                var embed = new LocalEmbedBuilder()
                    .WithTitle($"Help for {module.Name}")
                    .WithDescription("The following commands are available")
                    .WithFooter($"To get specific command help use {this.Context.Prefix}help <command name>");

                foreach (var command in CommandUtilities.EnumerateAllCommands(module).GroupBy(x => x.FullAliases.First()))
                    embed.AddField(command.Skip(1).Any() ? $"{command.Key} ({command.Count()})" : command.Key, command.First().Description ?? command.First().Module.Description ?? "unknown");

                return Ok(embed.Build());
            }

            [Command("", "command")]
            public Task<CommandResult> HelpAsync([Remainder, Description("The name of the command you want help for"), Name("command")] Command command)
            {
                var allCommands = this.commandService.GetAllCommands()
                    .Where(x => x.FullAliases.Any(y => y == command.FullAliases.First()))
                    .OrderBy(x => x.Parameters.Count);

                static string GetParameters(Command command)
                {
                    var parameterStrings = command.Parameters.Select(x =>
                    {
                        var param = string.Empty;
                        if (x.IsMultiple)
                            param += $"{x.Name}, ...";
                        else if (x.IsRemainder)
                            param += $"{x.Name}...";
                        else
                            param += $"{x.Name}";

                        if (x.IsOptional)
                            return $"<{param}>";

                        return $"[{param}]";
                    });

                    return string.Join(" ", parameterStrings);
                }

                static string FormatCommand(HatarakuCommandContext context, string alias, string parameters)
                    => $"{context.Message.Content[..(context.Prefix.Length + 1)]}{alias} {parameters}";

                static IEnumerable<string> FormatUsage(HatarakuCommandContext context, IEnumerable<Command> commands)
                {
                    foreach (var command in commands)
                        yield return FormatCommand(context, command.FullAliases.First(), GetParameters(command));
                }

                static IEnumerable<string> FormatAliases(HatarakuCommandContext context, IEnumerable<Command> commands)
                {
                    foreach (var command in commands)
                    {
                        var parameterText = GetParameters(command);
                        foreach (var alias in command.FullAliases.Skip(1))
                            yield return FormatCommand(context, alias, parameterText);
                    }
                }

                static IEnumerable<string> FormatParameterInfo(IEnumerable<Command> commands)
                {
                    var parameters = commands.SelectMany(x => x.Parameters);

                    foreach (var parameter in parameters)
                        yield return $"**{parameter.Name} [{(parameter.IsOptional ? "Optional" : "Required")}]**\n{parameter.Description ?? "unknown"}";
                }

                var embed = new LocalEmbedBuilder()
                    .WithTitle($"Help for {command.Name}")
                    .WithDescription(command.Description ?? command.Module.Description ?? "unknown")
                    .AddField("Usage", string.Join("\n", FormatUsage(this.Context, allCommands)))
                    .AddField("Aliases", string.Join("\n", FormatAliases(this.Context, allCommands)))
                    .AddField("Parameters", string.Join("\n", FormatParameterInfo(allCommands)));

                return Ok(embed.Build());
            }
        }
    }
}
