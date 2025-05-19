using System;
using System.Runtime.CompilerServices;

namespace EmptyBox.Construction.Machines;

/// <summary>
///     Контракт между контейнером и запущенной в нём службой.
/// </summary>
public interface IStateMachineContract
{
    /// <summary>
    ///     Текущее состояние машины.
    /// </summary>
    protected IState State => throw new NotImplementedException();

    /// <summary>
    ///     Изменяет состояние машины. 
    /// </summary>
    /// <typeparam name="SQ">
    ///     Тип нового состояния.
    /// </typeparam>
    /// <returns>
    ///     Обновлённое состояние машины.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected sealed SQ Switch<SQ>()
        where SQ : class, IState, new()
    {
        return Switch(new SQ());
    }

    /// <summary>
    ///     Изменяет состояние машины. 
    /// </summary>
    /// <typeparam name="SQ">
    ///     Тип нового состояния.
    /// </typeparam>
    /// <param name="newState">
    ///     Новое состояние машины.
    /// </param>
    /// <returns>
    ///     Обновлённое состояние машины.
    /// </returns>
    protected SQ Switch<SQ>(SQ newState)
        where SQ : class, IState
    {
        throw new NotImplementedException();
    }
}
