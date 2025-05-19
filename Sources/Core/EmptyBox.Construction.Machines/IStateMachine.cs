using System;

namespace EmptyBox.Construction.Machines;

/// <summary>
///     Представление контейнера службы.
/// </summary>
public interface IStateMachine : IStateMachineContract
{
    IState IStateMachineContract.State => State;

    /// <summary>
    ///     Событие, возникающее при смене состояния машины.
    /// </summary>
    public event StateSwitchedEventHandler? StateSwitched;

    /// <inheritdoc cref="IStateMachineContract.State"/>
    public new IState State { get; }
    /// <summary>
    ///     Текущий контракт службы, исполняемый машиной состояний.
    /// </summary>
    public Type Contract { get; }
}