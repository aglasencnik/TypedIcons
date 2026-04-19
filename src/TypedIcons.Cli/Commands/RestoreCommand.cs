using Spectre.Console.Cli;
using TypedIcons.Cli.Services;

namespace TypedIcons.Cli.Commands;

public class RestoreCommand(IconService iconService) : AsyncCommand<RestoreCommand.Settings>
{
    public class Settings : GlobalSettings
    {
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var result = await iconService.RestoreIconsAsync(cancellationToken);
        return result ? 0 : -1;
    }
}