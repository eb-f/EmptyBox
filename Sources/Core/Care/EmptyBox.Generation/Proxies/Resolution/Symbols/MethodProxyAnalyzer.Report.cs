using EmptyBox.Generation.Proxies.Resolution.Symbols.Qualification;

using Microsoft.CodeAnalysis;

using System.Collections.Immutable;

namespace EmptyBox.Generation.Proxies.Resolution.Symbols;

internal partial class MethodProxyAnalyzer
{
    public readonly struct Report
    {
        public required Target Target { get; init; }
        public required RefKind RefKind { get; init; }
        public required ImmutableArray<ITypeParameterSymbol> ExternalProxyMethodTypeParameters { get; init; }
        public required ImmutableArray<ITypeParameterSymbol> InternalProxyMethodTypeParameters { get; init; }
        public required MethodServiceAnalyzer.Report? ServiceReport { get; init; }
        public required MethodQualificationAnalyzer.Report? QualificationReport { get; init; }
    }
}
