using EmptyBox.Construction.Machines;
using EmptyBox.Presentation.Permissions;

using System;

namespace EmptyBox.Application.Services;

/// <summary>
///     Представление службы.
/// </summary>
/// <typeparam name="SQ">
///     Представление состояния службы.
/// </typeparam>
public interface IService<out SQ> : IQualified<SQ>, IStateMachineContract
    where SQ : class, IState
{
    Type IQualified<SQ>.Qualification => State.GetType();
}
