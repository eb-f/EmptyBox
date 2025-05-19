using EmptyBox.Generation.Diagnostics;

using System.Collections.Immutable;

namespace EmptyBox.Generation.Proxies.Resolution.Symbols;

internal partial class TypeProxyAnalyzer
{
    public readonly struct Report
    {
        public required SymbolCache Symbols { get; init; }
        public required PreliminaryReport Preliminary { get; init; }
        public required bool IsAsyncExternalProxyRequired { get; init; }
        public required ImmutableArray<MethodProxyAnalyzer.Report> MethodReports { get; init; }
        public required ImmutableArray<SymbolDiagnosticRecord> DiagnosticMessages { get; init; }

    }
}
