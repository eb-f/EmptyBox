using EmptyBox.Generation.Proxies.Resolution.Symbols;
using EmptyBox.Generation.Proxies.Resolution.Symbols.Qualification;

using Microsoft.CodeAnalysis;

namespace EmptyBox.Generation.Proxies.Construction.Syntaxes.CSharp;

internal abstract partial class MethodProxyBuilder
{
    public readonly struct Context
    {
        public required Accessibility Accessibility { get; init; }
        public required SymbolCache Symbols { get; init; }
        public required INamedTypeSymbol TargetType { get; init; }
        public required TypeQualificationAnalyzer.Report? QualificationReport { get; init; }
        public required MethodProxyAnalyzer.Report Report { get; init; }
        public required MethodQualificationAnalyzer.SingleQualificationReport? QualificationContext { get; init; }
    }
}