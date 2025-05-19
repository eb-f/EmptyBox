using Microsoft.CodeAnalysis;

using System.Collections.Generic;

namespace EmptyBox.Generation.Extensions;

internal static class NamedTypeSymbolExtensions
{
    public static IEnumerable<ITypeSymbol> EnumerateAllTypeArguments(this INamedTypeSymbol symbol)
    {
        foreach (ITypeSymbol argument in symbol.TypeArguments)
        {
            yield return argument;
        }

        INamedTypeSymbol? containingType = symbol.ContainingType;

        while (containingType != null)
        {
            foreach (ITypeSymbol argument in containingType.TypeArguments)
            {
                yield return argument;
            }

            containingType = containingType.ContainingType;
        }
    }
}
