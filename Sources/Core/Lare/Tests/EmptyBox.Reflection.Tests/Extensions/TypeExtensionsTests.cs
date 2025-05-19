using EmptyBox.Reflection.Extensions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace EmptyBox.Reflection.Tests.Extensions;

[TestClass]
public sealed class TypeExtensionsTests
{
    [TestMethod]
    public void ReplaceGenericArguments()
    {
        Assert.AreEqual(typeof(List<object>), typeof(List<string>).MakeConstructedGenericTypeLike(typeof(IEnumerable<object>)));

        Type DOESNT_MATTER_INVARIANT;

        #region Проверка ошибочных аргументов
        {
            DOESNT_MATTER_INVARIANT = typeof(IEnumerable<object>);
            ImmutableArray<Type> simpleErrorArguments =
            [
                typeof(void*),
                typeof(delegate*<void>),
                typeof(int),
                typeof(StringComparison),
                typeof(Span<>),
                typeof(IEnumerable),
                typeof(Array),
                typeof(IEnumerable<>),
                typeof(string[]),
                typeof(List<>),
                typeof(Action),
                typeof(Func<>)
            ];

            foreach (Type type in simpleErrorArguments)
            {
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => type.MakeConstructedGenericTypeLike(DOESNT_MATTER_INVARIANT));
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => DOESNT_MATTER_INVARIANT.MakeConstructedGenericTypeLike(type));
            }
        }
        #endregion
    }
}
