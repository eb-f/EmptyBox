using Microsoft.CodeAnalysis;

using System;
using System.Collections.Immutable;
using System.Threading;

namespace EmptyBox.Generation.Extensions;

/// <summary>
///     Содержит методы расширения <see cref="IncrementalValuesProvider{TValues}"/> для поддержки LINQ запросов.
/// </summary>
internal static class IncrementalValuesProviderExtensions
{
    public static IncrementalValuesProvider<T> Select<S, T>(this IncrementalValuesProvider<S> source, Func<(S Value, CancellationToken CancellationToken), T> transformation)
    {
        return source.Select((value, token) => transformation((value, token)));
    }

    public static IncrementalValuesProvider<T> Select<S, T>(this IncrementalValuesProvider<S> source, Func<S, T> transformation)
    {
        return source.Select((value, token) => transformation(value));
    }

    public static IncrementalValuesProvider<T> SelectMany<S, M, T>(this IncrementalValuesProvider<S> source, Func<S?, IncrementalValueProvider<M>> collectionSelector, Func<S, M, T> resultSelector)
    {
        return source.Combine(collectionSelector(default))
                     .Select((tuple, token) => resultSelector(tuple.Left, tuple.Right));
    }

    public static IncrementalValuesProvider<S> Where<S>(this IncrementalValuesProvider<S> source, Func<(S Value, CancellationToken CancellationToken), bool> predicate)
    {
        return source.SelectMany((item, c) => predicate((item, c))
                                            ? [item]
                                            : ImmutableArray<S>.Empty);
    }
}
