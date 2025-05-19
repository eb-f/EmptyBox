using Microsoft.CodeAnalysis;

using System.Collections.Immutable;

namespace EmptyBox.Generation.Proxies.Resolution.Symbols.Qualification;

internal partial class TypeQualificationAnalyzer
{
    public readonly struct Report
    {
        public required ITypeParameterSymbol QualifierParameter { get; init; }
        public required ImmutableArray<ITypeParameterSymbol> ParametersExceptQualifier { get; init; }
        public required ImmutableArray<ITypeParameterSymbol> LikewiseQualifiedParameters { get; init; }
    }
}
