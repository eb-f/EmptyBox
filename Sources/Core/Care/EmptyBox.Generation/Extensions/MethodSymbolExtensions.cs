using Microsoft.CodeAnalysis;

using System.Collections.Generic;

namespace EmptyBox.Generation.Extensions;

internal static class MethodSymbolExtensions
{
    public static IEnumerable<ITypeSymbol> EnumerateAllTypeArguments(this IMethodSymbol symbol)
    {
        foreach (ITypeSymbol argument in symbol.TypeArguments)
        {
            yield return argument;
        }

        IMethodSymbol? containingMethod = symbol.ContainingSymbol as IMethodSymbol;

        while (containingMethod != null)
        {
            foreach (ITypeSymbol argument in containingMethod.TypeArguments)
            {
                yield return argument;
            }

            containingMethod = containingMethod.ContainingSymbol as IMethodSymbol;
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

    public static IEnumerable<IMethodSymbol> EnumerateAllImplementedMethods(this IMethodSymbol symbol)
    {
        foreach (IMethodSymbol explicitImplementation in symbol.ExplicitInterfaceImplementations)
        {
            yield return explicitImplementation;
        }

        IMethodSymbol? current = symbol.OverriddenMethod;

        while (current != null)
        {
            yield return current;

            foreach (IMethodSymbol explicitImplementation in current.ExplicitInterfaceImplementations)
            {
                yield return explicitImplementation;
            }

            current = current.OverriddenMethod;
        }
    }
}
