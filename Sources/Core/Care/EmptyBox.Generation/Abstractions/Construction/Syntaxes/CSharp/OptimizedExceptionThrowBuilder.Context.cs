using EmptyBox.Generation.Proxies;
using EmptyBox.Generation.Writers.CSharp;

using Microsoft.CodeAnalysis;

using System;

namespace EmptyBox.Generation.Abstractions.Construction.Syntaxes.CSharp;

internal partial class OptimizedExceptionThrowBuilder
{
    public readonly struct Context
    {
        public required SymbolCache Symbols { get; init; }
        public required INamedTypeSymbol Target { get; init; }
        public ITypeSymbol? ReturnType { get; init; }
        public Action<CSharpWriter>? InnerExceptionWriter { get; init; }
        public Action<CSharpWriter>? MessageWriter { get; init; }
        public TypePresentationOptions TypePresentation { get; init; }
        public TypeTransformationOptions TypeTransformation { get; init; }
        public MethodReturnParameterUsage ReturnParameterUsage { get; init; }
    }
}
