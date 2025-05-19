using System;

namespace EmptyBox.Presentation.Permissions;

public class InvalidQualificationException(string? message = default, Exception? innerException = null) : InvalidOperationException(message, innerException)
{
    public InvalidQualificationException() : this(default, null) { }
}