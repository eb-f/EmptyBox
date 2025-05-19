using EmptyBox.Generation.Proxies.Resolution.Symbols;

using System.Collections.Immutable;

namespace EmptyBox.Generation.Proxies.Construction.Syntaxes.CSharp;

internal abstract partial class TypeProxyBuilder
{
    public readonly struct Context
    {
        public required SymbolCache Symbols { get; init; }
        public required bool IsAsyncExternalProxyRequired { get; init; }
        public required TypeProxyAnalyzer.PreliminaryReport Report { get; init; }
        public required ImmutableArray<MethodProxyAnalyzer.Report> MethodReports { get; init; }
    }
}
