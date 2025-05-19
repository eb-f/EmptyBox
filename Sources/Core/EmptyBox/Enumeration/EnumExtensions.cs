using System;
using System.Runtime.CompilerServices;

namespace EmptyBox.Enumeration;

public static class EnumExtensions
{
    /// <summary>
    ///     Проверяет наличие всех флагов в наборе.
    /// </summary>
    /// <typeparam name="E">
    ///     Представление перечисления.
    /// </typeparam>
    /// <param name="set">
    ///     Исходный набор флагов.
    /// </param>
    /// <param name="flag">
    ///     Набор флагов, наличие которых проверяется.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> в случае наличия всех указанных флагов в наборе, иначе - <see langword="false"/>.
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     Перечисление данного типа не поддерживается.
    /// </exception>
    /// <remarks>
    ///     Поведение функции аналогично <see cref="Enum.HasFlag(Enum)"/>.
    /// </remarks>
    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool Has<E>(this E set, E flag)
        where E : unmanaged, Enum
    {
        switch (sizeof(E))
        {
            case sizeof(byte):
                byte byte_flag = Unsafe.As<E, byte>(ref flag);
                return (Unsafe.As<E, byte>(ref set) & byte_flag) == byte_flag;
            case sizeof(ushort):
                ushort ushort_flag = Unsafe.As<E, ushort>(ref flag);
                return (Unsafe.As<E, ushort>(ref set) & ushort_flag) == ushort_flag;
            case sizeof(uint):
                uint uint_flag = Unsafe.As<E, uint>(ref flag);
                return (Unsafe.As<E, uint>(ref set) & uint_flag) == uint_flag;
            case sizeof(ulong):
                ulong ulong_flag = Unsafe.As<E, ulong>(ref flag);
                return (Unsafe.As<E, ulong>(ref set) & ulong_flag) == ulong_flag;
            default:
                throw new NotSupportedException();
        }
    }

    /// <summary>
    ///     Проверяет наличие хотя бы одного из флагов в наборе.
    /// </summary>
    /// <typeparam name="E">
    ///     Представление перечисления.
    /// </typeparam>
    /// <param name="set">
    ///     Исходный набор флагов.
    /// </param>
    /// <param name="flag">
    ///     Набор флагов, наличие которых проверяется.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> в случае наличия хотя бы одного флага в наборе, иначе - <see langword="false"/>.
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///     Перечисление данного типа не поддерживается.
    /// </exception>
    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool HasAny<E>(this E set, E flag)
        where E : unmanaged, Enum
    {
        switch (sizeof(E))
        {
            case sizeof(byte):
                byte byte_flag = Unsafe.As<E, byte>(ref flag);
                return (Unsafe.As<E, byte>(ref set) & byte_flag) != 0;
            case sizeof(ushort):
                ushort ushort_flag = Unsafe.As<E, ushort>(ref flag);
                return (Unsafe.As<E, ushort>(ref set) & ushort_flag) != 0;
            case sizeof(uint):
                uint uint_flag = Unsafe.As<E, uint>(ref flag);
                return (Unsafe.As<E, uint>(ref set) & uint_flag) != 0;
            case sizeof(ulong):
                ulong ulong_flag = Unsafe.As<E, ulong>(ref flag);
                return (Unsafe.As<E, ulong>(ref set) & ulong_flag) != 0;
            default:
                throw new NotSupportedException();
        }
    }
}
