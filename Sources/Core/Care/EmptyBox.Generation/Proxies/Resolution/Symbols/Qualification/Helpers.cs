using EmptyBox.Generation.Extensions;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace EmptyBox.Generation.Proxies.Resolution.Symbols.Qualification;

internal static class Helpers
{
    [return: NotNullIfNotNull(nameof(data))]
    public static ITypeSymbol? GetAttributeTypeArgument(IMethodSymbol method, AttributeData? data)
    {
        if (data == null)
        {
            return null;
        }
        else if (data.AttributeClass?.IsGenericType ?? false)
        {
            return data.AttributeClass.TypeArguments[0];
        }
        else
        {
            Dictionary<string, ITypeSymbol> availableParameters1 = method.EnumerateAllTypeArguments()
                                                                         .Select(static symbol => (symbol.Name, symbol.GetSealedConstraintOrThis()))
                                                                         .ToDictionary(static pair => pair.Name, static pair => pair.Item2);

            Dictionary<string, ITypeSymbol> availableParameters = method.ContainingType
                                                                        .TypeParameters
                                                                        .Concat(method.TypeParameters)
                                                                        .Zip(method.ContainingType
                                                                                   .TypeArguments
                                                                                   .Concat(method.TypeParameters
                                                                                                 .Select(TypeSymbolExtensions.GetSealedConstraintOrThis)),
                                                                             static (parameter, argument) => (parameter.Name, argument))
                                                                        .ToDictionary(static pair => pair.Name, static pair => pair.argument);

            return data.ConstructorArguments[0].Value switch
            {
                string genericParameterName => availableParameters[genericParameterName],
                INamedTypeSymbol fillingType => data.ConstructorArguments.Length == 1
                                              ? fillingType
                                              : fillingType.ConstructedFrom
                                                           .Construct(data.ConstructorArguments[1]
                                                                          .Values
                                                                          .Select(name => availableParameters[(string)name.Value!])
                                                                          .ToArray()),
                _ => throw new NotSupportedException(),
            };
        }
    }
}
