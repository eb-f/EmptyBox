using System;
using System.Collections.Immutable;

namespace EmptyBox.Presentation.Permissions;

/// <summary>
///     Атрибут, указывающий тип квалификатора, для которого вызов метода, отмеченного данным атрибутом, разрешён.
/// </summary>
/// <remarks>
///     Приводит к генерации метода-расширения, позволяющего вызывать отмеченный метод только для объекта с соответствующим квалификатором.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class QualifiedAttribute(Type qualifier, params string[] typeParameterNames) : Attribute
{
    /// <summary>
    ///     Аргументы типа квалификатора.
    /// </summary>
    public ImmutableArray<string> TypeParameterNames { get; } = [.. typeParameterNames];
    /// <summary>
    ///     Квалификатор доступа к методу.
    /// </summary>
    public Type? QualifierType { get; } = qualifier;

    public QualifiedAttribute(string typeParameterName) : this(null!, [typeParameterName]) { }
}

/// <summary>
///     Атрибут, указывающий тип квалификатора, для которого вызов метода, отмеченного данным атрибутом, разрешён.
/// </summary>
/// <typeparam name="Q">
///     Квалификатор доступа к методу.
/// </typeparam>
/// <remarks>
///     Приводит к генерации метода-расширения, позволяющего вызывать отмеченный метод только для объекта с соответствующим квалификатором.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class QualifiedAttribute<Q>() : QualifiedAttribute(typeof(Q))
    where Q : class, IQualifier;