using System.ComponentModel;
using Spectre.Console.Cli;
using TypedIcons.Cli.Services;

namespace TypedIcons.Cli.Commands;

public class InitCommand(InitializationService initializationService) : AsyncCommand<InitCommand.Settings>
{
    public class Settings : GlobalSettings
    {
        [CommandOption("-y|--yes")]
        [Description("Confirm optional prompts")]
        [DefaultValue(false)]
        public bool ConfirmPrompts { get; init; } = false;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings,
        CancellationToken cancellationToken)
    {
        var success = await initializationService.InitializeAsync(settings.ConfirmPrompts, cancellationToken);
        return success ? 0 : -1;
    }
}