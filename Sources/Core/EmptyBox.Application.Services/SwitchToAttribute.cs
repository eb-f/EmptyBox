using EmptyBox.Construction.Machines;

using System;
using System.Collections.Immutable;

namespace EmptyBox.Application.Services;

/// <summary>
///     Атрибут, указывающий на состояние службы после выполнения метода, результат которого помечен данным атрибутом.
/// </summary>
/// <remarks>
///     <para>
///         Данный атрибут можно использовать лишь единожды. Для возможности перехода в различные состояния в качестве аргумента конструктора укажите базовый тип для всех состояний, в которые может перейти служба по завершению исполнения данного метода.
///     </para>
///     <para>
///         В случае переопределения атрибута над реализуемым или наследуемым методом, в качестве аргумента следует указывать тип, представимый типом, указанным у базового метода в данном атрибуте.
///     </para>
/// </remarks>
[AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = true)]
public class SwitchToAttribute(Type qualifier, params string[] typeParameterNames) : Attribute
{
    public Type? QualifierType { get; } = qualifier;
    public ImmutableArray<string> TypeParameterNames { get; } = [.. typeParameterNames];

    public SwitchToAttribute(string typeParameterName) : this(null!, [typeParameterName]) { }
}

/// <summary>
///     Атрибут, указывающий на состояние службы после выполнения метода, результат которого помечен данным атрибутом.
/// </summary>
/// <typeparam name="SQ">
///     Новое состояние службы.
/// </typeparam>
/// <remarks>
///     <para>
///         Данный атрибут можно использовать лишь единожды. Для возможности перехода в различные состояния в качестве аргумента типа укажите базовый тип для всех состояний, в которые может перейти служба по завершению исполнения данного метода.
///     </para>
///     <para>
///         В случае переопределения атрибута над реализуемым или наследуемым методом, в качестве аргумента следует указывать тип, представимый типом, указанным у базового метода в данном атрибуте.
///     </para>
/// </remarks>
[AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = true)]
public class SwitchToAttribute<SQ>() : SwitchToAttribute(typeof(SQ))
    where SQ : class, IState;