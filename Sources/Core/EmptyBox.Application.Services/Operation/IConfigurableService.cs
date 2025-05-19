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
    /// <returns>
    ///     Служба в сконфигурированном состоянии.
    /// </returns>
    [Qualified<SI>, Qualified<SC>]
    [return: SwitchTo(typeof(SC<>), nameof(C))]
    protected void Configure<C>(C configuration)
        where C : CB
    {
        _ = Switch(new SC<C>() { Configuration = configuration });
    }
}
