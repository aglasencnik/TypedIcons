using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace TypedIcons.Tests.Helpers;

internal sealed class InMemoryAdditionalText(string path, string content) : AdditionalText
{
    public override string Path { get; } = path;
    public override SourceText? GetText(CancellationToken ct = default)
        => SourceText.From(content, Encoding.UTF8);
}