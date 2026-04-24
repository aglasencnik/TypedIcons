using System.Text.Json;
using CliWrap;
using Spectre.Console;
using TypedIcons.Core;
using TypedIcons.Core.Models;

namespace TypedIcons.Cli.Services;

public class InitializationService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

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

        var configPath = Path.Combine(currentDirectory, TypedIconsDefaults.ConfigFileName);
        if (!File.Exists(configPath))
        {
            await File.WriteAllTextAsync(
                configPath,
                JsonSerializer.Serialize(new IconConfig(), _jsonSerializerOptions),
                cancellationToken
            );
            AnsiConsole.MarkupLine("[green]Created typedicons.json[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]typedicons.json already exists[/]");
            return true;
        }

        var cacheDir = Path.Combine(Directory.GetCurrentDirectory(), "obj", TypedIconsDefaults.CacheFolderName);
        if (!Directory.Exists(cacheDir))
            Directory.CreateDirectory(cacheDir);

        var cachePath = Path.Combine(cacheDir, TypedIconsDefaults.CacheFileName);
        await File.WriteAllTextAsync(cachePath, JsonSerializer.Serialize(new IconCache(), _jsonSerializerOptions),
            cancellationToken);
        AnsiConsole.MarkupLine("[green]Created cache file[/]");

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

            AnsiConsole.MarkupLine("\n[blue]Also add to[/] [grey]Components/_Imports.razor[/][blue]:[/]");
            AnsiConsole.Write(new Panel("@using TypedIcons")
                .BorderColor(Color.Grey)
                .Header("Add to Components/_Imports.razor manually"));

            return true;
        }

        AnsiConsole.MarkupLine("[grey]Installing package...[/]");

        CommandResult result;

        try
        {
            result = await CliWrap.Cli.Wrap("dotnet")
                .WithArguments("add package TypedIcons.Generator --prerelease")
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

        AnsiConsole.MarkupLine("[green]Source generator package installed successfully[/]");

        var addImport = confirmPrompts ||
                        await AnsiConsole.ConfirmAsync(
                            "Do you want to automatically add [grey]@using TypedIcons[/] to [grey]Components/_Imports.razor[/]?",
                            cancellationToken: cancellationToken);

        if (!addImport)
        {
            AnsiConsole.MarkupLine("\n[blue]Add manually to[/] [grey]Components/_Imports.razor[/][blue]:[/]");
            AnsiConsole.Write(new Panel("@using TypedIcons")
                .BorderColor(Color.Grey)
                .Header("Add to Components/_Imports.razor manually"));
        }
        else
        {
            var importsPath = Path.Combine(currentDirectory, "Components", "_Imports.razor");

            if (!File.Exists(importsPath))
            {
                AnsiConsole.MarkupLine($"[yellow]Components/_Imports.razor not found, skipping.[/]");
                AnsiConsole.MarkupLine("\n[blue]Add manually:[/]");
                AnsiConsole.Write(new Panel("@using TypedIcons")
                    .BorderColor(Color.Grey)
                    .Header("Add to Components/_Imports.razor manually"));
            }
            else
            {
                var importsContent = await File.ReadAllTextAsync(importsPath, cancellationToken);
                const string usingDirective = "@using TypedIcons";

                if (importsContent.Split('\n').Select(l => l.Trim()).Any(l => l == usingDirective))
                {
                    AnsiConsole.MarkupLine("[yellow]@using TypedIcons already present in _Imports.razor[/]");
                }
                else
                {
                    await File.AppendAllTextAsync(importsPath,
                        $"{Environment.NewLine}{usingDirective}", cancellationToken);
                    AnsiConsole.MarkupLine("[green]Added @using TypedIcons to Components/_Imports.razor[/]");
                }
            }
        }

        AnsiConsole.MarkupLine("[green]TypedIcons initialized successfully[/]");

        return true;
    }
}