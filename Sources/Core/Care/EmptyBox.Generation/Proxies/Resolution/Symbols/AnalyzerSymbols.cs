using EmptyBox.Generation.Extensions;

using Microsoft.CodeAnalysis;

using System.Collections.Immutable;

namespace EmptyBox.Generation.Proxies.Resolution.Symbols;

internal readonly struct AnalyzerSymbols(Compilation compilation)
{
    public INamedTypeSymbol IException { get; } = compilation.GetTypeByMetadataName("EmptyBox.Execution.IException")!;
    public INamedTypeSymbol Exception { get; } = compilation.GetTypeByMetadataName("System.Exception")!;
    public INamedTypeSymbol InvalidOperationException { get; } = compilation.GetTypeByMetadataName("System.InvalidOperationException")!;
    public ImmutableArray<INamedTypeSymbol> Task { get; } = [.. compilation.EnumerateGenericTypesByMetadataName("System.Threading.Tasks.Task", 0, 1)!];
    public ImmutableArray<INamedTypeSymbol> ValueTask { get; } = [.. compilation.EnumerateGenericTypesByMetadataName("System.Threading.Tasks.ValueTask", 0, 1)!];
    public INamedTypeSymbol Void { get; } = compilation.GetSpecialType(SpecialType.System_Void);
}
