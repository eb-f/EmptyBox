using System;

namespace EmptyBox.Presentation.Permissions;

/// <summary>
///     Исключительная ситуация, возникающая при попытке взаимодействовать с объектом, имеющим недопустимую для производимого действия квалификацию.
/// </summary>
/// <param name="message">
///     Пояснительное сообщение.
/// </param>
/// <param name="innerException">
///     Исключительная ситуация, предшествующая данной.
/// </param>
public class InvalidQualificationException(string? message = default, Exception? innerException = null) : InvalidOperationException(message, innerException)
{
    public InvalidQualificationException() : this(default, null) { }
}