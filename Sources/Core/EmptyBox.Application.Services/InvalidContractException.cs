using System;

namespace EmptyBox.Application.Services;

/// <summary>
///     Исключительная ситуация, возникающая при попытке использовать недопустимый контракт.
/// </summary>
/// <param name="message">
///     Пояснительное сообщение.
/// </param>
/// <param name="innerException">
///     Исключительная ситуация, предшествующая данной.
/// </param>
public class InvalidContractException(string? message = default, Exception? innerException = null) : InvalidOperationException(message, innerException)
{
    public InvalidContractException() : this(default, null) { }
}