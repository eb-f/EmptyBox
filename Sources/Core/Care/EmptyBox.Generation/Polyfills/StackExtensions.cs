using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EmptyBox.Generation.Polyfills;

internal static class StackExtensions
{
    public static bool TryPop<T>(this Stack<T> stack, [MaybeNullWhen(false)] out T value)
    {
        if (stack.Count == 0)
        {
            value = default;

            return false;
        }
        else
        {
            value = stack.Pop();

            return true;
        }
    }
}
