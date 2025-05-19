using EmptyBox.Construction.Machines;

using System;
using System.Collections.Immutable;

namespace EmptyBox.Application.Services;

/// <summary>
///     Атрибут, указывающий на состояние службы после выполнения метода, результат которого помечен данным атрибутом.
/// </summary>
/// <remarks>
///     Данный атрибут можно использовать лишь единожды. Для возможности перехода в различные состояния в качестве параметра укажите базовый тип для всех состояний, в которые может перейти служба по завершению исполнения данного метода.
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
///     Данный атрибут можно использовать лишь единожды. Для возможности перехода в различные состояния в качестве <typeparamref name="SQ"/> укажите базовый тип для всех состояний, в которые может перейти служба по завершению исполнения данного метода.
/// </remarks>
[AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = true)]
public class SwitchToAttribute<SQ>() : SwitchToAttribute(typeof(SQ))
    where SQ : class, IState;