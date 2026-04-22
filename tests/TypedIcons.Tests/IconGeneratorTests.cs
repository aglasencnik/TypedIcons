using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using TypedIcons.Generator;
using TypedIcons.Tests.Helpers;

namespace TypedIcons.Tests;

public class IconGeneratorTests
{
    [Fact]
    public void DebugGenerator()
    {
        var generator = new IconGenerator();

        var configuration = File.ReadAllText("TestData/typedicons.json");
        var cache = File.ReadAllText("TestData/typedicons.cache.json");

        var additionalFiles = ImmutableArray.Create<AdditionalText>(
            new InMemoryAdditionalText("typedicons.json", configuration),
            new InMemoryAdditionalText("typedicons.cache.json", cache)
        );
        
        var compilation = CSharpCompilation.Create(
            assemblyName: nameof(IconGeneratorTests),
            references: [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]
        );

        var driver = CSharpGeneratorDriver
            .Create(generator)
            .AddAdditionalTexts(additionalFiles);

        var result = driver.RunGenerators(compilation).GetRunResult();

        var generatedFiles = result.GeneratedTrees;
        var diagnostics = result.Diagnostics;
        var exception = result.Results[0].Exception;

        Assert.Null(exception);
        Assert.NotEmpty(generatedFiles);
    }
}