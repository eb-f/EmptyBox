using System;

namespace EmptyBox.Application.Services;

public class ContractViolationException(string? message = default, Exception? innerException = null) : Exception(message, innerException)
{
    public ContractViolationException() : this(default, null) { }
}
