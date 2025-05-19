using Microsoft.CodeAnalysis;

using System;

namespace EmptyBox.Generation.Writers.CSharp;

internal readonly struct TypeTransformationOptions
{
    public Func<ITypeSymbol, ITypeSymbol>? TypeSubstitution { get; init; }
    public Action<CSharpWriter, ITypeSymbol>? NameSubstitution { get; init; }
}
