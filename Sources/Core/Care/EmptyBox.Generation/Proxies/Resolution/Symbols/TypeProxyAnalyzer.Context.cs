using EmptyBox.Generation.Diagnostics;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

namespace EmptyBox.Generation.Proxies.Resolution.Symbols;

internal partial class TypeProxyAnalyzer
{
    public readonly struct Context()
    {
        public required SemanticModel SemanticModel { get; init; }
        public required SymbolCache Symbols { get; init; }
        public required List<SymbolDiagnosticRecord> DiagnosticMessages { get; init; }
        public required Target Target { get; init; }
    }
}
