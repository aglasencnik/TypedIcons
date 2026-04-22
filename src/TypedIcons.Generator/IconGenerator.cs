using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using TypedIcons.Core;
using TypedIcons.Core.Models;

namespace TypedIcons.Generator;

[Generator]
public class IconGenerator : IIncrementalGenerator
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource("IconData.g.cs", SourceText.From(CodeTemplates.IconDataClass, Encoding.UTF8)));

        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource("Icon.g.cs", SourceText.From(CodeTemplates.IconComponent, Encoding.UTF8)));

        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource("Icons.g.cs", SourceText.From(CodeTemplates.EmptyIconsClass, Encoding.UTF8)));
        
        var provider = context.AdditionalTextsProvider
            .Where(f =>
                Path.GetFileName(f.Path) is TypedIconsDefaults.ConfigFileName or TypedIconsDefaults.CacheFileName)
            .Collect();

        context.RegisterSourceOutput(provider, Generate);
    }

    private void Generate(SourceProductionContext context, ImmutableArray<AdditionalText> files)
    {
        var configFileText = files
            .FirstOrDefault(f => Path.GetFileName(f.Path) is TypedIconsDefaults.ConfigFileName)?
            .GetText(context.CancellationToken)?
            .ToString();

        if (configFileText is null)
            return;

        var cacheFileText = files
            .FirstOrDefault(f => Path.GetFileName(f.Path) is TypedIconsDefaults.CacheFileName)?
            .GetText(context.CancellationToken)?
            .ToString();
        
        if (cacheFileText is null)
            return;

        var config = JsonSerializer.Deserialize<IconConfig>(configFileText, _jsonSerializerOptions);
        var cache = JsonSerializer.Deserialize<IconCache>(cacheFileText, _jsonSerializerOptions);
        
        if (config is null || cache is null)
            return;

        var iconSets = config.Icons.GroupBy(x => x.Set);
        foreach (var iconSet in iconSets)
        {
            var iconSetName = iconSet.Key;
            var pascalIconSetName = ToPascalCase(iconSetName);

            var fieldStringBuilder = new StringBuilder();

            foreach (var iconReference in iconSet)
            {
                var iconifyName = $"{iconReference.Set}:{iconReference.Name}";
                if (!cache.Icons.TryGetValue(iconifyName, out var iconCache) || iconCache is null)
                    continue;
                
                XDocument? doc;
                try
                {
                    doc = XDocument.Parse(iconCache);
                }
                catch
                {
                    continue;
                }

                var svg = doc.Root;
                if (svg is null)
                    continue;
                
                var viewBox = svg.Attribute("viewBox")?.Value ?? "0 0 24 24";
                var width = svg.Attribute("width")?.Value ?? "1em";
                var height = svg.Attribute("height")?.Value ?? "1em";
                var content = string.Concat(svg?.Nodes() ?? []).Trim();

                fieldStringBuilder.AppendLine(
                    CodeTemplates.IconSetClassFieldTemplate
                        .Replace("{{iconName}}", ToPascalCase(iconReference.Name))
                        .Replace("{{svgContent}}", Escape(content))
                        .Replace("{{viewBox}}", Escape(viewBox))
                        .Replace("{{width}}", Escape(width))
                        .Replace("{{height}}", Escape(height))
                );
            }
            
            var iconSetClass = CodeTemplates.IconSetClassTemplate
                .Replace("{{className}}", pascalIconSetName)
                .Replace("{{fields}}", fieldStringBuilder.ToString());
            
            context.AddSource($"{pascalIconSetName}.Icons.g.cs", SourceText.From(iconSetClass, Encoding.UTF8));
        }
    }

    private static string ToPascalCase(string input)
    {
        if  (string.IsNullOrEmpty(input))
            return input;
        
        var parts = input.Split(['-', '_', ' ', '.'], StringSplitOptions.RemoveEmptyEntries);
        var result = new StringBuilder();

        foreach (var part in parts)
        {
            if (part.Length == 0)
                continue;

            if (part.Length == 1)
            {
                result.Append(char.ToUpperInvariant(part[0]));
            }
            else
            {
                result.Append(char.ToUpperInvariant(part[0]));
                result.Append(part.Substring(1).ToLowerInvariant());
            }
        }

        return result.ToString();
    }
    
    private static string Escape(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        
        return input
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\r", "")
            .Replace("\n", "");
    }
}