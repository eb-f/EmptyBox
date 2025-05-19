using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EmptyBox.Generation.Polyfills;

internal static class QueueExtensions
{
    public static bool TryDequeue<T>(this Queue<T> queue, [MaybeNullWhen(false)] out T? value)
    {
        if (queue.Count == 0)
        {
            value = default;

            return false;
        }
        else
        {
            value = queue.Dequeue();

            return true;
        }
    }
}
