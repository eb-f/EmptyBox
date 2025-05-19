using EmptyBox.Generation.Proxies.Resolution.Symbols;
using EmptyBox.Generation.Proxies.Resolution.Symbols.Qualification;

using Microsoft.CodeAnalysis;

namespace EmptyBox.Generation.Proxies;

internal class SymbolCache(Compilation compilation)
{
    public Compilation Compilation { get; } = compilation;
    public AnalyzerSymbols Base { get; } = new(compilation);
    public QualificationSymbols Qualification { get; } = new(compilation);
}
