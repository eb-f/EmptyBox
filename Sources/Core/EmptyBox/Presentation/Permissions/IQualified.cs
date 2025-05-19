using System;

namespace EmptyBox.Presentation.Permissions;

/// <summary>
///     Контракт объекта с квалификацией.
/// </summary>
/// <typeparam name="Q">
///     Тип квалификации.
/// </typeparam>
public interface IQualified<out Q>
    where Q : IQualifier
{
    /// <summary>
    ///     Квалификация экземпляра объекта.
    /// </summary>
    protected Type Qualification => typeof(Q);
}
