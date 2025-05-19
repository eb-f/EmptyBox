using EmptyBox.Presentation.Permissions;

namespace EmptyBox.Construction.Machines;

/// <summary>
///     Контракт состояния.
/// </summary>
public interface IState : IQualifier
{
    /// <summary>
    ///     При допустимости, пополняет состояние <paramref name="state"/> данными из текущего состояния.
    /// </summary>
    /// <typeparam name="S">
    ///     Представление пополняемого данными состояния.
    /// </typeparam>
    /// <param name="state">
    ///     Пополняемое данными состояние.
    /// </param>
    public void Map<S>(S state)
        where S : class, IState;
}
