namespace EmptyBox.Construction.Machines;

/// <summary>
///     Обработчик события, возникающего при смене состояния машины.
/// </summary>
/// <param name="stateMachine">
///     Машина состояний, сообщившая об изменении.
/// </param>
/// <param name="previousState">
///     Состояние, предшествующее текущему.
/// </param>
/// <param name="currentState">
///     Текущее состояние машины.
/// </param>
public delegate void StateSwitchedEventHandler(IStateMachine stateMachine, IState previousState, IState currentState);
