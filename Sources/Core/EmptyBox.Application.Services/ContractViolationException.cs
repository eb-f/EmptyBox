using System;

namespace EmptyBox.Application.Services;

/// <summary>
///     Исключительная ситуация, возникающая при нарушении контракта службой.
/// </summary>
/// <param name="message">
///     Пояснительное сообщение.
/// </param>
/// <param name="innerException">
///     Исключительная ситуация, предшествующая данной.
/// </param>
public class ContractViolationException(string? message = default, Exception? innerException = null) : Exception(message, innerException)
{
    public ContractViolationException() : this(default, null) { }
}
