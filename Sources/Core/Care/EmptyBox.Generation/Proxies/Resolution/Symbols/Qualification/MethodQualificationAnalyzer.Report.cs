using System.Collections.Immutable;

namespace EmptyBox.Generation.Proxies.Resolution.Symbols.Qualification;

internal partial class MethodQualificationAnalyzer
{
    public readonly struct Report
    {
        public required bool IsProxyRequired { get; init; }
        public required ImmutableArray<SingleQualificationReport> Qualifications { get; init; }
    }
}
