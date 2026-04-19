using CliWrap;
using Spectre.Console;
using TypedIcons.Core;

namespace TypedIcons.Cli.Services;

public class InitializationService
{
    public async Task<bool> InitializeAsync(bool confirmPrompts, CancellationToken cancellationToken)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        
        var csproj = Directory.GetFiles(currentDirectory, "*.csproj").FirstOrDefault();
        if (csproj is null)
        {
            AnsiConsole.MarkupLine("[red]No .csproj found in current directory[/]");
            return false;
        }
        
        AnsiConsole.MarkupLine($"[green]Found project:[/] {Path.GetFileName(csproj)}");

        var metadataPath = Path.Combine(currentDirectory, TypedIconsDefaults.ConfigFileName);
        if (!File.Exists(metadataPath))
        {
            await File.WriteAllTextAsync(metadataPath, "{ \"icons\": [] }", cancellationToken);
            AnsiConsole.MarkupLine("[green]Created typedicons.json[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]typedicons.json already exists[/]");
            return true;
        }

        var installGenerator = confirmPrompts ||
            await AnsiConsole.ConfirmAsync("Do you want to install the TypedIcons source generator?",
                cancellationToken: cancellationToken);

        if (!installGenerator)
        {
            AnsiConsole.MarkupLine("\n[blue]Install manually:[/]");
            AnsiConsole.Write(new Panel(@"<ItemGroup>
  <PackageReference Include=""TypedIcons.Generator"" Version=""x.y.z"" />
</ItemGroup>")
                .BorderColor(Color.Grey)
                .Header("Add to your .csproj manually"));
            return true;
        }
        
        AnsiConsole.MarkupLine("[grey]Installing package...[/]");

        CommandResult result;
        
        try
        {
            result = await CliWrap.Cli.Wrap("dotnet")
                .WithArguments("package add TypedIcons.Generator --prerelease")
                .ExecuteAsync(cancellationToken);
        }
        catch
        {
            AnsiConsole.MarkupLine("[red]Failed to install package[/]");
            return false;
        }

        if (result.ExitCode != 0)
        {
            AnsiConsole.MarkupLine("[red]Failed to install package[/]");
            return false;
        }

        AnsiConsole.MarkupLine("[green]Package installed successfully[/]");
        AnsiConsole.MarkupLine("[green]TypedIcons initialized successfully[/]");
        
        return true;
    }
}