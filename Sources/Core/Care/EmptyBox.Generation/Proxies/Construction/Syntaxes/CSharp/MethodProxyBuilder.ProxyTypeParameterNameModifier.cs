using EmptyBox.Generation.Writers.CSharp;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;
using System.Collections.Immutable;

namespace EmptyBox.Generation.Proxies.Construction.Syntaxes.CSharp;

internal abstract partial class MethodProxyBuilder
{
    protected sealed class ProxyTypeParameterNameModifier(INamedTypeSymbol container, params IEnumerable<ITypeParameterSymbol> proxiedParameters)
    {
        private readonly INamedTypeSymbol Container = container;
        private readonly ImmutableArray<ITypeParameterSymbol> ProxiedParameters = [.. proxiedParameters];

        public void Format(CSharpWriter writer, ITypeSymbol type)
        {
            if (type is ITypeParameterSymbol parameter && SymbolEqualityComparer.Default.Equals(parameter.ContainingSymbol, Container) && ProxiedParameters.Contains(parameter, SymbolEqualityComparer.Default))
            {
                writer.Append("Proxy");
            }

            writer.Append(type.Name);
        }
    }
}
