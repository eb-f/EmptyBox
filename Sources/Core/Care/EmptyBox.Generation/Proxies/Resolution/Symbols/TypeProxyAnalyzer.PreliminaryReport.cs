using EmptyBox.Generation.Proxies.Resolution.Symbols.Qualification;

namespace EmptyBox.Generation.Proxies.Resolution.Symbols;

internal partial class TypeProxyAnalyzer
{
    public readonly struct PreliminaryReport
    {
        public required Target Target { get; init; }
        public required TypeQualificationAnalyzer.Report? QualificationReport { get; init; }
    }
}
