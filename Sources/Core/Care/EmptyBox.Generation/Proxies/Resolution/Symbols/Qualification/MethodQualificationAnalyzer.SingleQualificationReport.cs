using Microsoft.CodeAnalysis;

namespace EmptyBox.Generation.Proxies.Resolution.Symbols.Qualification;

internal partial class MethodQualificationAnalyzer
{
    public readonly struct SingleQualificationReport
    {
        public required ITypeSymbol Qualification { get; init; }
    }
}
