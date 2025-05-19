using Microsoft.CodeAnalysis;

using System.Collections.Immutable;

namespace EmptyBox.Generation.Proxies.Construction.Syntaxes.CSharp;

internal sealed partial class FileProxyBuilder
{
    public readonly struct Context
    {
        public required TypeProxyBuilder.Context Type { get; init; }
        public ImmutableArray<INamespaceSymbol> IncludedNamespaces { get; init; }
    }
}
