using EmptyBox.Generation.Polyfills;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Linq;

namespace EmptyBox.Generation.Extensions;

public static class TypeSymbolExtensions
{
    public static ITypeSymbol GetSealedConstraintOrThis(this ITypeSymbol type)
    {
        return type is ITypeParameterSymbol parameter
             ? parameter.ConstraintTypes.FirstOrDefault(static c => c.IsSealed) ?? type
             : type;
    }

    public static bool ContainsTypeArgument(this ITypeSymbol type, ITypeSymbol item)
    {
        Queue<ITypeSymbol> checkQueue = [];
        HashSet<ITypeSymbol> @checked = [];

        ITypeSymbol? current = type;

        do
        {
            if (SymbolEqualityComparer.Default.Equals(type, item))
            {
                return true;
            }
            else if (current is INamedTypeSymbol named)
            {
                @checked.Add(named);

                foreach (ITypeSymbol argument in named.TypeArguments.Distinct<ITypeSymbol>(SymbolEqualityComparer.Default))
                {
                    if (!@checked.Contains(argument))
                    {
                        checkQueue.Enqueue(argument);
                    }
                }
            }
            else if (current is ITypeParameterSymbol parameter)
            {
                @checked.Add(parameter);

                foreach (ITypeSymbol argument in parameter.ConstraintTypes)
                {
                    checkQueue.Enqueue(argument);
                }
            }
        } while (checkQueue.TryDequeue(out current));

        return false;
    }

    public static IEnumerable<INamedTypeSymbol> EnumerateBaseTypes(this ITypeSymbol type)
    {
        INamedTypeSymbol? baseType = type.BaseType;

        while (baseType != null)
        {
            yield return baseType;

            baseType = baseType.BaseType;
        }
    }

    public static IEnumerable<INamedTypeSymbol> EnumerateContainerTypes(this ITypeSymbol type)
    {
        INamedTypeSymbol? containingType = type.ContainingType;

        while (containingType != null)
        {
            yield return containingType;

            containingType = containingType.ContainingType;
        }
    }

    /// <remarks>
    ///     Не учитывает вариантность параметров типа.
    /// </remarks>
    public static bool IsAssignableTo(this ITypeSymbol type, ITypeSymbol location)
    {
        if (SymbolEqualityComparer.Default.Equals(type, location))
        {
            return true;
        }
        else if (location.TypeKind == TypeKind.Interface)
        {
            return type.AllInterfaces.Contains(location, SymbolEqualityComparer.Default);
        }
        else if (!type.IsRefLikeType)
        {
            return type.EnumerateBaseTypes().Any(baseType => SymbolEqualityComparer.Default.Equals(baseType, location));
        }

        return false;
    }
}
