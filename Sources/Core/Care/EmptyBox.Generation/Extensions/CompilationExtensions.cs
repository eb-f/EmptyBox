using Microsoft.CodeAnalysis;

using System.Collections.Generic;

namespace EmptyBox.Generation.Extensions;

public static class CompilationExtensions
{
    public static IEnumerable<INamedTypeSymbol?> EnumerateGenericTypesByMetadataName(this Compilation compilation, string name, uint minimalTypeParametersCount, uint maximumTypeParametersCount)
    {
        if (minimalTypeParametersCount == 0)
        {
            yield return compilation.GetTypeByMetadataName(name);
            minimalTypeParametersCount++;
        }

        for (uint count = minimalTypeParametersCount; count <= maximumTypeParametersCount; count++)
        {
            yield return compilation.GetTypeByMetadataName($"{name}`{count}");
        }
    }
}
