using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace EmptyBox.Presentation.Containers;

/// <summary>
///    Слабая ссылка на значение.
/// </summary>
/// <typeparam name="T">
///     Представление значения по ссылке.
/// </typeparam>
/// <param name="value">
///     Значение, на которое будет указывать слабая ссылка.
/// </param>
/// <remarks>
///     При отсутствии иных ссылок на значение, значение будет своевременно удалено из памяти сборщиком мусора.
/// </remarks>
public sealed class WeakRef<T>(T? value)
    where T : class
{
    private GCHandle Handle = GCHandle.Alloc(value, GCHandleType.Weak);

    /// <summary>
    ///     Создаёт слабую ссылку и инициализирует её значением <see langword="null"/>.
    /// </summary>
    public WeakRef() : this(null) { }

    /// <summary>
    ///     Ссылка на значение.
    /// </summary>
    public unsafe ref T? Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (Handle.IsAllocated)
            {
                GC.KeepAlive(this);
                return ref Unsafe.AsRef<T?>((void*)(nint)Handle);
            }
            else
            {
                return ref Unsafe.NullRef<T?>();
            }
        }
    }

    /// <summary>
    ///     Освобождает ресурсы, связанные со слабой ссылкой.
    /// </summary>
    ~WeakRef()
    {
        Handle.Free();
    }
}
