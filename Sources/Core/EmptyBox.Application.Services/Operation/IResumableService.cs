using EmptyBox.Construction.Machines;
using EmptyBox.Presentation.Permissions;

namespace EmptyBox.Application.Services.Operation;

/// <summary>
///     Контракт возобновляемой службы.
/// </summary>
/// <typeparam name="SQ">
///     Представление состояния службы.
/// </typeparam>
/// <typeparam name="RSQB">
///     Базовое представление состояния, из которого возможно возобновление.
/// </typeparam>
public partial interface IResumableService<out SQ, RSQB> : IService<SQ>
    where SQ : class, IState
    where RSQB : class, IState
{
    /// <summary>
    ///     Получает текущее состояние машины.
    /// </summary>
    /// <returns>
    ///     Текущее возобновляемое состояние машины.
    /// </returns>
    /// <remarks>
    ///     Этот метод должен быть <see langword="get"/> частью свойства <see langword="State"/>, однако на данный момент, квалифицированный доступ к свойствам не поддерживается.
    /// </remarks>
    [Qualified(nameof(RSQB))]
    protected RSQB get_State() => (RSQB)State;

    /// <summary>
    ///     Переводит службу в состояние <typeparamref name="RSQ"/>.
    /// </summary>
    /// <typeparam name="RSQ">
    ///     Представление состояния для возобновления.
    /// </typeparam>
    /// <param name="resumedState">
    ///     Состояние для возобновления.
    /// </param>
    [Qualified<SI>]
    [return: SwitchTo(nameof(RSQ))]
    protected void Resume<RSQ>(RSQ resumedState)
        where RSQ : class, RSQB
    {
        _ = Switch(resumedState);
    }

    /// <summary>
    ///     Перезапускает службу в состоянии <see cref="SI"/>.
    /// </summary>
    [Qualified(nameof(RSQB))]
    [return: SwitchTo<SI>]
    protected void Reset()
    {
        _ = Switch(SI.Instance);
    }
}
