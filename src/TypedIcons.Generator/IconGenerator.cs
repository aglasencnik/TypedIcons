using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace TypedIcons.Generator;

[Generator]
public class IconGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource("IconData.g.cs", SourceText.From(CodeTemplates.IconDataClass, Encoding.UTF8)));
        
        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource("Icon.g.cs", SourceText.From(CodeTemplates.IconComponent, Encoding.UTF8)));
        
        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource("Icons.g.cs", SourceText.From(CodeTemplates.EmptyIconsClass, Encoding.UTF8)));
    }
}