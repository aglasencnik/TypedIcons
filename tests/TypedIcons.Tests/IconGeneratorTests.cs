using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using TypedIcons.Generator;

namespace TypedIcons.Tests;

public class IconGeneratorTests
{
    [Fact]
    public void DebugGenerator()
    {
        var generator = new IconGenerator();
        
        var compilation = CSharpCompilation.Create(
            assemblyName: nameof(IconGeneratorTests),
            references: [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]
        );

        var driver = CSharpGeneratorDriver.Create(generator);

        var result = driver.RunGenerators(compilation).GetRunResult();

        var generatedFiles = result.GeneratedTrees;
        var diagnostics = result.Diagnostics;
        var exception = result.Results[0].Exception;

        Assert.Null(exception);
        Assert.NotEmpty(generatedFiles);
    }
}