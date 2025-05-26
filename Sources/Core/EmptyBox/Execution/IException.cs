using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace EmptyBox.Execution;

[SuppressMessage("Style", "IDE0036:Order modifiers", Justification = "Ложноположительное срабатывание диагностики")]
file static class ExceptionFieldAccessExtensions
{
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_innerException")]
    private static extern ref Exception? ref_InnerException(this Exception exception);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_innerExceptions")]
    private static extern ref Exception[] ref_InnerExceptions(this AggregateException exception);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_message")]
    public static extern ref string? ref_Message(this Exception exception);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_paramName")]
    public static extern ref string? ref_ParamName(this ArgumentException exception);

    public static void set_InnerException(this Exception exception, Exception innerException)
    {
        exception.ref_InnerException() = innerException;

        if (exception is AggregateException aggregate)
        {
            aggregate.ref_InnerExceptions() = [innerException];
        }
    }
}

/// <summary>
///     Набор методов для сокращения расхода бюджета метода в рамках встраивания вызова JIT-компилятором.
/// </summary>
public interface IException
{
    [StackTraceHidden, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Fill(Exception exception, string? message, Exception? innerException)
    {
        if (message != default)
        {
            exception.ref_Message() = message;
        }

        if (innerException != null)
        {
            exception.set_InnerException(innerException);
        }
    }

    [StackTraceHidden, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Fill(ArgumentException exception, string? message, Exception? innerException, string? parameterName)
    {
        Fill(exception, message, innerException);

        if (parameterName != default)
        {
            exception.ref_ParamName() = parameterName;
        }
    }

    /// <summary>
    ///     Создаёт и выбрасывает исключение <typeparamref name="E"/>.
    /// </summary>
    /// <typeparam name="E">
    ///     Представление исключительной ситуации.
    /// </typeparam>
    /// <param name="message">
    ///     Пояснительное сообщение.
    /// </param>
    /// <param name="innerException">
    ///     Исключительная ситуация, предшествующая данной.
    /// </param>
    [StackTraceHidden, DebuggerHidden]
    [DoesNotReturn]
    public static void Throw<E>(string? message = default, Exception? innerException = null)
        where E : Exception, new()
    {
        E exception = new();
        Fill(exception, message, innerException);

        throw exception;
    }

    /// <summary>
    ///     Создаёт и выбрасывает исключение <typeparamref name="E"/>.
    /// </summary>
    /// <typeparam name="E">
    ///     Представление исключительной ситуации.
    /// </typeparam>
    /// <param name="message">
    ///     Пояснительное сообщение.
    /// </param>
    /// <param name="innerException">
    ///     Исключительная ситуация, предшествующая данной.
    /// </param>
    /// <param name="parameterName">
    ///     Имя параметра, значение которого стало поводом для возникновения исключительной ситуации.
    /// </param>
    [StackTraceHidden, DebuggerHidden]
    [DoesNotReturn]
    public static void Throw<E>(string? message = default, Exception? innerException = null, string? parameterName = null)
        where E : ArgumentException, new()
    {
        E exception = new();
        Fill(exception, message, innerException, parameterName);

        throw exception;
    }

    /// <summary>
    ///     Выбрасывает исключение <typeparamref name="E"/>.
    /// </summary>
    /// <typeparam name="E">
    ///     Представление исключительной ситуации.
    /// </typeparam>
    /// <typeparam name="R">
    ///     Представление возвращаемого значения функцией.
    /// </typeparam>
    /// <param name="exception">
    ///     Экземпляр исключения.
    /// </param>
    [StackTraceHidden, DebuggerHidden]
    [DoesNotReturn]
    public static void Throw<E>(E exception)
        where E : Exception
    {
        throw exception;
    }

    /// <summary>
    ///     Создаёт и выбрасывает исключение <typeparamref name="E"/>.
    /// </summary>
    /// <typeparam name="E">
    ///     Представление исключительной ситуации.
    /// </typeparam>
    /// <typeparam name="R">
    ///     Представление возвращаемого значения функцией.
    /// </typeparam>
    /// <param name="message">
    ///     Пояснительное сообщение.
    /// </param>
    /// <param name="innerException">
    ///     Исключительная ситуация, предшествующая данной.
    /// </param>
    /// <returns>
    ///     Выполнение метода завершается ошибкой.
    /// </returns>
    /// <remarks>
    ///     Данный метод предназначен для использования:
    ///     <list type="bullet">
    ///         <item>в функциях, совместно с лексемами <see langword="return"/> и <see langword="=>"/>;</item>
    ///         <item>в операторах <see langword="switch"/>, <see langword="??"/>, <see langword="? :"/>.</item>
    ///     </list>
    /// </remarks>
    [StackTraceHidden, DebuggerHidden]
    [DoesNotReturn]
    public static R Throw<E, R>(string? message = default, Exception? innerException = null)
        where E : Exception, new()
        where R : allows ref struct
    {
        E exception = new();
        Fill(exception, message, innerException);

        throw exception;
    }

    /// <summary>
    ///     Создаёт и выбрасывает исключение <typeparamref name="E"/>.
    /// </summary>
    /// <typeparam name="E">
    ///     Представление исключительной ситуации.
    /// </typeparam>
    /// <typeparam name="R">
    ///     Представление возвращаемого значения функцией.
    /// </typeparam>
    /// <param name="message">
    ///     Пояснительное сообщение.
    /// </param>
    /// <param name="innerException">
    ///     Исключительная ситуация, предшествующая данной.
    /// </param>
    /// <param name="parameterName">
    ///     Имя параметра, значение которого стало поводом для возникновения исключительной ситуации.
    /// </param>
    /// <returns>
    ///     Выполнение метода завершается ошибкой.
    /// </returns>
    /// <remarks>
    ///     Данный метод предназначен для использования:
    ///     <list type="bullet">
    ///         <item>в функциях, совместно с лексемами <see langword="return"/> и <see langword="=>"/>;</item>
    ///         <item>в операторах <see langword="switch"/>, <see langword="??"/>, <see langword="? :"/>.</item>
    ///     </list>
    /// </remarks>
    [StackTraceHidden, DebuggerHidden]
    [DoesNotReturn]
    public static R Throw<E, R>(string? message = default, Exception? innerException = null, string? parameterName = null)
        where E : ArgumentException, new()
        where R : allows ref struct
    {
        E exception = new();
        Fill(exception, message, innerException, parameterName);

        throw exception;
    }

    /// <summary>
    ///     Выбрасывает исключение <typeparamref name="E"/>.
    /// </summary>
    /// <typeparam name="E">
    ///     Представление исключительной ситуации.
    /// </typeparam>
    /// <typeparam name="R">
    ///     Представление возвращаемого значения функцией.
    /// </typeparam>
    /// <param name="exception">
    ///     Экземпляр исключения.
    /// </param>
    /// <returns>
    ///     Выполнение метода завершается ошибкой.
    /// </returns>
    /// <remarks>
    ///     Данный метод предназначен для использования:
    ///     <list type="bullet">
    ///         <item>в функциях, совместно с лексемами <see langword="return"/> и <see langword="=>"/>;</item>
    ///         <item>в операторах <see langword="switch"/>, <see langword="??"/>, <see langword="? :"/>.</item>
    ///     </list>
    /// </remarks>
    [StackTraceHidden, DebuggerHidden]
    [DoesNotReturn]
    public static R Throw<E, R>(E exception)
        where E : Exception
        where R : allows ref struct
    {
        throw exception;
    }


    /// <summary>
    ///     Создаёт и выбрасывает исключение <typeparamref name="E"/>.
    /// </summary>
    /// <typeparam name="E">
    ///     Представление исключительной ситуации.
    /// </typeparam>
    /// <typeparam name="R">
    ///     Представление возвращаемого значения функцией.
    /// </typeparam>
    /// <param name="message">
    ///     Пояснительное сообщение.
    /// </param>
    /// <param name="innerException">
    ///     Исключительная ситуация, предшествующая данной.
    /// </param>
    /// <returns>
    ///     Выполнение метода завершается ошибкой.
    /// </returns>
    /// <remarks>
    ///     Данный метод предназначен для использования:
    ///     <list type="bullet">
    ///         <item>в функциях, совместно с лексемами <see langword="return ref"/> и <see langword="=> ref"/>.</item>
    ///     </list>
    /// </remarks>
    [StackTraceHidden, DebuggerHidden]
    [DoesNotReturn]
    public static ref R ThrowRef<E, R>(string? message = default, Exception? innerException = null)
        where E : Exception, new()
        where R : allows ref struct
    {
        E exception = new();
        Fill(exception, message, innerException);

        throw exception;
    }

    /// <summary>
    ///     Создаёт и выбрасывает исключение <typeparamref name="E"/>.
    /// </summary>
    /// <typeparam name="E">
    ///     Представление исключительной ситуации.
    /// </typeparam>
    /// <typeparam name="R">
    ///     Представление возвращаемого значения функцией.
    /// </typeparam>
    /// <param name="message">
    ///     Пояснительное сообщение.
    /// </param>
    /// <param name="innerException">
    ///     Исключительная ситуация, предшествующая данной.
    /// </param>
    /// <param name="parameterName">
    ///     Имя параметра, значение которого стало поводом для возникновения исключительной ситуации.
    /// </param>
    /// <returns>
    ///     Выполнение метода завершается ошибкой.
    /// </returns>
    /// <remarks>
    ///     Данный метод предназначен для использования:
    ///     <list type="bullet">
    ///         <item>в функциях, совместно с лексемами <see langword="return ref"/> и <see langword="=> ref"/>.</item>
    ///     </list>
    /// </remarks>
    [StackTraceHidden, DebuggerHidden]
    [DoesNotReturn]
    public static ref R ThrowRef<E, R>(string? message = default, Exception? innerException = null, string? parameterName = null)
        where E : ArgumentException, new()
        where R : allows ref struct
    {
        E exception = new();
        Fill(exception, message, innerException, parameterName);

        throw exception;
    }

    /// <summary>
    ///     Выбрасывает исключение <typeparamref name="E"/>.
    /// </summary>
    /// <typeparam name="E">
    ///     Представление исключительной ситуации.
    /// </typeparam>
    /// <typeparam name="R">
    ///     Представление возвращаемого значения функцией.
    /// </typeparam>
    /// <param name="exception">
    ///     Экземпляр исключения.
    /// </param>
    /// <returns>
    ///     Выполнение метода завершается ошибкой.
    /// </returns>
    /// <remarks>
    ///     Данный метод предназначен для использования:
    ///     <list type="bullet">
    ///         <item>в функциях, совместно с лексемами <see langword="return ref"/> и <see langword="=> ref"/>.</item>
    ///     </list>
    /// </remarks>
    [StackTraceHidden, DebuggerHidden]
    [DoesNotReturn]
    public static ref R ThrowRef<E, R>(E exception)
        where E : Exception
        where R : allows ref struct
    {
        throw exception;
    }
}
