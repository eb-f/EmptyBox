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

    [StackTraceHidden, DebuggerHidden]
    [DoesNotReturn]
    public static void Throw<E>(string? message = default, Exception? innerException = null)
        where E : Exception, new()
    {
        E exception = new();
        Fill(exception, message, innerException);

        throw exception;
    }

    [StackTraceHidden, DebuggerHidden]
    [DoesNotReturn]
    public static void Throw<E>(string? message = default, Exception? innerException = null, string? parameterName = null)
        where E : ArgumentException, new()
    {
        E exception = new();
        Fill(exception, message, innerException, parameterName);

        throw exception;
    }

    [StackTraceHidden, DebuggerHidden]
    [DoesNotReturn]
    public static void Throw<E>(E exception)
        where E : Exception
    {
        throw exception;
    }

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

    [StackTraceHidden, DebuggerHidden]
    [DoesNotReturn]
    public static R Throw<E, R>(E exception)
        where E : Exception
        where R : allows ref struct
    {
        throw exception;
    }

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

    [StackTraceHidden, DebuggerHidden]
    [DoesNotReturn]
    public static ref R ThrowRef<E, R>(E exception)
        where E : Exception
        where R : allows ref struct
    {
        throw exception;
    }
}
