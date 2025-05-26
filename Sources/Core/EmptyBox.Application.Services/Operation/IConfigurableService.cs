using EmptyBox.Construction.Machines;
using EmptyBox.Presentation.Permissions;

namespace EmptyBox.Application.Services.Operation;

/// <summary>
///     Контракт конфигурируемой службы.
/// </summary>
/// <typeparam name="SQ">
///     Представление состояния службы.
/// </typeparam>
/// <typeparam name="CB">
///     Базовое представление конфигурации службы.
/// </typeparam>
public partial interface IConfigurableService<out SQ, in CB> : IService<SQ>
    where SQ : class, IState
{
    /// <summary>
    ///     Конфигурирует управляемую службу.
    /// </summary>
    /// <typeparam name="C">
    ///     Представление конфигурации службы.
    /// </typeparam>
    /// <param name="configuration">
    ///     Экземпляр конфигурации службы.
    /// </param>
    /// <returns>
    ///     Служба в сконфигурированном состоянии.
    /// </returns>
    /// <remarks>
    ///     Тип <see cref="SC{C}"/> инвариантен относительно параметра типа, в зависимости от сценария использования может потребоваться явное указание параметра типа <typeparamref name="C"/>.
    /// </remarks>
    [Qualified<SI>, Qualified<SC>]
    [return: SwitchTo(typeof(SC<>), nameof(C))]
    protected void Configure<C>(C configuration)
        where C : CB
    {
        _ = Switch(new SC<C>() { Configuration = configuration });
    }
}
