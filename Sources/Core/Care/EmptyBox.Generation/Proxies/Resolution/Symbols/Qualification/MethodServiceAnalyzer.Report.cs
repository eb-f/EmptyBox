using Microsoft.CodeAnalysis;

namespace EmptyBox.Generation.Proxies.Resolution.Symbols.Qualification;

internal partial class MethodServiceAnalyzer
{
    public readonly struct Report
    {
        public required bool IsAsync { get; init; }
        public required ITypeSymbol SwitchToQualification { get; init; }
        public required INamedTypeSymbol OverriddenReturnType { get; init; }
    }
}
