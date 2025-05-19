using EmptyBox.Generation.Extensions;

using Microsoft.CodeAnalysis;

using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

namespace EmptyBox.Generation.Proxies.Construction.Syntaxes.CSharp;

internal abstract partial class MethodProxyBuilder
{
    protected sealed class TypeSubstitution
    {
        private readonly FrozenDictionary<ITypeSymbol, ITypeSymbol> Map;

        public TypeSubstitution(IReadOnlyDictionary<ITypeSymbol, ITypeSymbol> map)
        {
            Map = map.ToFrozenDictionary(SymbolEqualityComparer.Default);
        }

        public TypeSubstitution(params IEnumerable<(ITypeSymbol? Parameter, ITypeSymbol? Argument)> pairs)
        {
            Map = pairs.Where(static pair => pair.Parameter != null && pair.Argument != null)
                      !.ToFrozenDictionary(static ITypeSymbol (pair) => pair.Parameter!,
                                           static ITypeSymbol (pair) => pair.Argument!,
                                           SymbolEqualityComparer.Default);
        }

        public TypeSubstitution(ITypeSymbol? parameter, ITypeSymbol? argument) : this([(parameter, argument)])
        {

        }

        public ITypeSymbol Replace(ITypeSymbol type)
        {
            return Map.TryGetValue(type, out ITypeSymbol? substitute)
                 ? substitute
                 : type.GetSealedConstraintOrThis();
        }
    }
}