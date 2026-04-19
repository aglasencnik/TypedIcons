using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TypedIcons.Cli.Services;

namespace TypedIcons.Cli.Commands;

public class AddCommand(IconService iconService) : AsyncCommand<AddCommand.Settings>
{
    public class Settings : GlobalSettings
    {
        [CommandArgument(0, "<name>")] 
        [Description("The name of the icon (<set>:<icon>)")]
        public string Name { get; init; } = string.Empty;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(settings.Name))
        {
            AnsiConsole.MarkupLine("[red]Icon name is required (<set>:<icon>)[/]");
            return -1;
        }

        var result = await iconService.AddIconAsync(settings.Name, cancellationToken);
        return result ? 0 : -1;
    }
}