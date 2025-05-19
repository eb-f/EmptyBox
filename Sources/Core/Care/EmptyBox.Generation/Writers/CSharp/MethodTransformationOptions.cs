using Microsoft.CodeAnalysis;

using System;
using System.Collections.Immutable;

namespace EmptyBox.Generation.Writers.CSharp;

internal readonly struct MethodTransformationOptions
{
    public Action<CSharpWriter, IMethodSymbol>? NameSubstitution { get; init; }
    public Action<CSharpWriter, ImmutableArray<IParameterSymbol>, MethodParameterWriterOptions>? ParameterSubstitution { get; init; }
    public Action<CSharpWriter, ImmutableArray<ITypeSymbol>, TypeWriterOptions>? TypeParameterSubstitution { get; init; }
    public TypeTransformationOptions? ReturnParameterTransformation { get; init; }
}
