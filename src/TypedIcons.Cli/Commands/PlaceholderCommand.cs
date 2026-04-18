using Spectre.Console;
using Spectre.Console.Cli;

namespace TypedIcons.Cli.Commands;

public class PlaceholderCommand : AsyncCommand<PlaceholderCommand.Settings>
{
    public class Settings : GlobalSettings
    {
    }

    protected override Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine("Hello, World!");
        return Task.FromResult(0);
    }
}