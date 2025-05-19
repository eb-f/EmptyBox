using Microsoft.CodeAnalysis;

using System.Collections.Immutable;

namespace EmptyBox.Generation.Diagnostics;

internal readonly struct SymbolDiagnosticRecord
{
    public required DiagnosticIdentifier Identifier { get; init; }
    public required ImmutableArray<SyntaxReference> SyntaxReferences { get; init; }
    public ImmutableArray<object> Arguments { get; init; }
}
