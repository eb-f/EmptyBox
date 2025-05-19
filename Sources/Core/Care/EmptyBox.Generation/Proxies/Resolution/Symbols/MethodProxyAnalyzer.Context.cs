using EmptyBox.Generation.Diagnostics;

using System.Collections.Generic;

namespace EmptyBox.Generation.Proxies.Resolution.Symbols;

internal partial class MethodProxyAnalyzer
{
    public readonly struct Context()
    {
        public required SymbolCache Symbols { get; init; }
        public required List<SymbolDiagnosticRecord> DiagnosticMessages { get; init; }
        public required TypeProxyAnalyzer.PreliminaryReport TypeReport { get; init; }
        public required Target Target { get; init; }
    }
}
