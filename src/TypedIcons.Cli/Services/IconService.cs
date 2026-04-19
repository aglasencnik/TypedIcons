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

    public async Task<bool> AddIconAsync(string iconifyName, CancellationToken cancellationToken)
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
            AnsiConsole.MarkupLine($"[yellow]{TypedIconsDefaults.ConfigFileName} is not found[/]");
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
        
        var cacheDir = Path.Combine(Directory.GetCurrentDirectory(), "obj", TypedIconsDefaults.CacheFolderName);
        if (!Directory.Exists(cacheDir))
            Directory.CreateDirectory(cacheDir);
        
        var cachePath = Path.Combine(cacheDir, TypedIconsDefaults.CacheFileName);
        var cacheContent = File.Exists(cachePath)
            ? await File.ReadAllTextAsync(cachePath, cancellationToken)
            : "{}";

        var cache = JsonSerializer.Deserialize<IconCache>(cacheContent, _jsonSerializerOptions);
        cache.Icons[iconifyName] = iconSvg;
        
        await File.WriteAllTextAsync(cachePath, JsonSerializer.Serialize(cache, _jsonSerializerOptions),
            cancellationToken);
        
        AnsiConsole.MarkupLine($"[green]Icon: '{iconifyName}' added successfully[/]");

        return true;
    }
}