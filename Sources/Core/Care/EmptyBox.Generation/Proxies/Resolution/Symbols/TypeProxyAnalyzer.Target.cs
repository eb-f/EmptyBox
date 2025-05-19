using Microsoft.CodeAnalysis;

namespace EmptyBox.Generation.Proxies.Resolution.Symbols;

internal partial class TypeProxyAnalyzer
{
    public readonly struct Target
    {
        public required bool IsPartial { get; init; }
        public required INamedTypeSymbol Symbol { get; init; }
    }
}
