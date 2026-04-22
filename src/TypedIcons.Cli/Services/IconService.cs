using System.Text.Json;
using Spectre.Console;
using TypedIcons.Core;
using TypedIcons.Core.Models;

namespace TypedIcons.Cli.Services;

public class IconService(IconifyService iconifyService, InitializationService initializationService)
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    public async Task<bool> AddIconAsync(string iconifyName, CancellationToken cancellationToken = default)
    {
        iconifyName = iconifyName.Trim();
        var iconNameParts = iconifyName.Split(':');
        if (iconNameParts.Length != 2 ||
            string.IsNullOrWhiteSpace(iconNameParts[0]) ||
            string.IsNullOrWhiteSpace(iconNameParts[1]))
        {
            AnsiConsole.MarkupLine($"[red]{iconifyName} is not a valid icon name[/]");
            return false;
        }

        var iconSet = iconNameParts[0];
        var iconName = iconNameParts[1];

        var configPath = Path.Combine(Directory.GetCurrentDirectory(), TypedIconsDefaults.ConfigFileName);
        if (!File.Exists(configPath))
        {
            AnsiConsole.MarkupLine($"[yellow]{TypedIconsDefaults.ConfigFileName} was not found[/]");
            var createConfigPrompt = await AnsiConsole.ConfirmAsync("Do you want to initialize the TypedIcons project?",
                cancellationToken: cancellationToken);

            if (!createConfigPrompt)
                return false;

            var initResult = await initializationService.InitializeAsync(false, cancellationToken);
            if (!initResult || !File.Exists(configPath))
                return false;
        }

        var configContent = await File.ReadAllTextAsync(configPath, cancellationToken);
        var config = JsonSerializer.Deserialize<IconConfig>(configContent, _jsonSerializerOptions);
        if (config is null)
        {
            AnsiConsole.MarkupLine($"[red]{TypedIconsDefaults.ConfigFileName} was not found or it couldn't be parsed[/]");
            return false;
        }

        if (config.Icons.Any(x =>
                string.Equals(x.Set, iconSet, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(x.Name, iconName, StringComparison.OrdinalIgnoreCase)))
        {
            AnsiConsole.MarkupLine($"[yellow]Icon: '{iconifyName}' already exists[/]");
            return false;
        }

        var iconSvg = await iconifyService.GetIconSvgAsync(iconSet, iconName, cancellationToken);
        if (iconSvg is null)
        {
            AnsiConsole.MarkupLine($"[red]Icon could not be retrieved[/]");
            return false;
        }

        config.Icons.Add(new IconReference
        {
            Set = iconSet,
            Name = iconName,
        });

        await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(config, _jsonSerializerOptions),
            cancellationToken);

        var cachePath = Path.Combine(Directory.GetCurrentDirectory(), "obj", TypedIconsDefaults.CacheFileName);
        var cacheContent = File.Exists(cachePath)
            ? await File.ReadAllTextAsync(cachePath, cancellationToken)
            : "{}";

        var cache = JsonSerializer.Deserialize<IconCache>(cacheContent, _jsonSerializerOptions) ?? new IconCache();
        cache.Icons[iconifyName] = iconSvg;

        await File.WriteAllTextAsync(cachePath, JsonSerializer.Serialize(cache, _jsonSerializerOptions),
            cancellationToken);

        AnsiConsole.MarkupLine($"[green]Icon: '{iconifyName}' added successfully[/]");

        return true;
    }

    public async Task<bool> RestoreIconsAsync(CancellationToken cancellationToken = default)
    {
        var configPath = Path.Combine(Directory.GetCurrentDirectory(), TypedIconsDefaults.ConfigFileName);
        if (!File.Exists(configPath))
        {
            AnsiConsole.MarkupLine($"[red]{TypedIconsDefaults.ConfigFileName} was not found[/]");
            return false;
        }

        var configContent = await File.ReadAllTextAsync(configPath, cancellationToken);
        var config = JsonSerializer.Deserialize<IconConfig>(configContent, _jsonSerializerOptions);
        if (config is null)
        {
            AnsiConsole.MarkupLine($"[red]{TypedIconsDefaults.ConfigFileName} was not found or it couldn't be parsed[/]");
            return false;
        }

        var cachePath = Path.Combine(Directory.GetCurrentDirectory(), "obj", TypedIconsDefaults.CacheFileName);
        var cacheContent = File.Exists(cachePath)
            ? await File.ReadAllTextAsync(cachePath, cancellationToken)
            : "{}";

        var cache = JsonSerializer.Deserialize<IconCache>(cacheContent, _jsonSerializerOptions) ?? new IconCache();

        await AnsiConsole.Status()
            .StartAsync("Restoring icons...", async ctx =>
            {
                foreach (var iconReference in config.Icons)
                {
                    var iconifyName = $"{iconReference.Set}:{iconReference.Name}";

                    if (cache.Icons.TryGetValue(iconifyName, out var existingIcon) &&
                        !string.IsNullOrWhiteSpace(existingIcon))
                        continue;

                    var iconSvg =
                        await iconifyService.GetIconSvgAsync(iconReference.Set, iconReference.Name, cancellationToken);

                    if (iconSvg is null)
                    {
                        AnsiConsole.MarkupLine($"[yellow]Icon: '{iconifyName}' could not be retrieved[/]");
                        continue;
                    }

                    cache.Icons[iconifyName] = iconSvg;

                    AnsiConsole.MarkupLine($"[gray]Icon: '{iconifyName}' restored[/]");
                }
            });

        await File.WriteAllTextAsync(cachePath, JsonSerializer.Serialize(cache, _jsonSerializerOptions),
            cancellationToken);

        AnsiConsole.MarkupLine("[green]Icons restored successfully[/]");

        return true;
    }
}