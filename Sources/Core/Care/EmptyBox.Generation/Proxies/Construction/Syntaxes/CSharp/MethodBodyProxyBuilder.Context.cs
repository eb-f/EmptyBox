using EmptyBox.Generation.Proxies.Resolution.Symbols;
using EmptyBox.Generation.Proxies.Resolution.Symbols.Qualification;
using EmptyBox.Generation.Writers.CSharp;

using Microsoft.CodeAnalysis;

using System.Collections.Immutable;

namespace EmptyBox.Generation.Proxies.Construction.Syntaxes.CSharp;

internal abstract partial class MethodBodyProxyBuilder
{
    public readonly struct Context
    {
        public required SymbolCache Symbols { get; init; }
        public required INamedTypeSymbol TargetType { get; init; }
        public required MethodProxyAnalyzer.Report Report { get; init; }
        public required ImmutableQueue<MethodBodyProxyBuilder> SequencedBuilders { get; init; }
        public required MethodQualificationAnalyzer.SingleQualificationReport? QualificationContext { get; init; }
        public required TypeWriterOptions OriginTypeWriterOptions { get; init; }
        public required TypeWriterOptions SwitchedTypeWriterOptions { get; init; }
        public required MethodWriterOptions MethodWriterOptions { get; init; }
        public required string ResultPrepend { get; init; }
    }
}
