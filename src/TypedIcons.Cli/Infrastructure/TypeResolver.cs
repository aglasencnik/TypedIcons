using Spectre.Console.Cli;

namespace TypedIcons.Cli.Infrastructure;

public sealed class TypeResolver(IServiceProvider provider) : ITypeResolver
{
    public object? Resolve(Type? type) => type == null ? null : provider.GetService(type);
}