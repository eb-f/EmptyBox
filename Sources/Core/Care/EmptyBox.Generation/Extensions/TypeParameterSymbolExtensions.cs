using Microsoft.CodeAnalysis;

namespace EmptyBox.Generation.Extensions;

internal static class TypeParameterSymbolExtensions
{
    public static bool HasAnyConstraint(this ITypeParameterSymbol symbol)
    {
        return symbol.ConstraintTypes.Length > 0
            || symbol.AllowsRefLikeType
            || symbol.HasConstructorConstraint
            || symbol.HasNotNullConstraint
            || symbol.HasReferenceTypeConstraint
            || symbol.HasUnmanagedTypeConstraint
            || symbol.HasValueTypeConstraint;
    }
}
