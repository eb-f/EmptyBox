using Microsoft.CodeAnalysis;

using System.Collections.Immutable;
using System.Linq;

namespace EmptyBox.Generation.Proxies.Resolution.Symbols;

internal partial class MethodProxyAnalyzer
{
    public readonly struct Target
    {
        /// <summary>
        ///     Метод, с которого начался анализ.
        /// </summary>
        public required IMethodSymbol Origin { get; init; }
        /// <summary>
        ///     Исходное определение метода.
        /// </summary>
        public required IMethodSymbol Symbol { get; init; }
        /// <summary>
        ///     Все методы от анализируемого до исходного.
        /// </summary>
        public required ImmutableArray<IMethodSymbol> Implementers { get; init; }
        /// <summary>
        ///     Из сигнатуры метода понятно, что ему нужен модификатор <see langword="unsafe"/>.
        /// </summary>
        public bool IsExactlyUnsafe => Symbol.ReturnType is IPointerTypeSymbol || Symbol.Parameters.Any(static parameter => parameter.Type is IPointerTypeSymbol);
    }
}
