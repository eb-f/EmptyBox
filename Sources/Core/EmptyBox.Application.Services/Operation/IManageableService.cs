using EmptyBox.Construction.Machines;
using EmptyBox.Presentation.Permissions;

using System.Threading;
using System.Threading.Tasks;

namespace EmptyBox.Application.Services.Operation;

/// <summary>
///     Контракт управляемой службы.
/// </summary>
/// <typeparam name="SQ">
///     Представление состояния управляемой службы.
/// </typeparam>
public partial interface IManageableService<out SQ> : IService<SQ>
    where SQ : class, IState
{
    /// <summary>
    ///     Запускает управляемую службу.
    /// </summary>
    /// <returns>
    ///     Служба в запущенном состоянии.
    /// </returns>
    [Qualified<SC>]
    [return: SwitchTo<ISL>]
    protected ValueTask Launch(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Останавливает управляемую службу.
    /// </summary>
    /// <returns>
    ///     Служба в сконфигурированном состоянии.
    /// </returns>
    [Qualified<ISL>]
    [return: SwitchTo<SC>]
    protected ValueTask Stop(CancellationToken cancellationToken = default);
}

/// <summary>
///     Расширенный контракт управляемой службы, позволяющий более точно описать состояние службы по завершению выполнения метода <see cref="IManageableService{SQ}.Launch(CancellationToken)"/>.
/// </summary>
/// <typeparam name="SQ">
///     Представление состояния управляемой службы.
/// </typeparam>
/// <typeparam name="SL">
///     Представление состояния запущенной службы.
/// </typeparam>
public partial interface IManageableService<out SQ, out SL> : IManageableService<SQ>
    where SQ : class, IState
    where SL : class, ISL
{
    [Qualified<SC>]
    [return: SwitchTo(nameof(SL))]
    abstract ValueTask IManageableService<SQ>.Launch(CancellationToken cancellationToken);
}
