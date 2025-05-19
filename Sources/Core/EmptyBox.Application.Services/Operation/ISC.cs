using EmptyBox.Construction.Machines;

namespace EmptyBox.Application.Services.Operation;

/// <summary>
///     Контракт состояния сконфигурированной службы.
/// </summary>
[State]
public interface ISC : IState;

/// <summary>
///     Контракт состояния сконфигурированной службы.
/// </summary>
/// <typeparam name="C">
///     Представление конфигурации службы.
/// </typeparam>
[State]
public interface ISC<C> : ISC
{
    /// <summary>
    ///     Конфигурация службы.
    /// </summary>
    public C Configuration { get; set; }
}

/// <summary>
///     Состояние сконфигурированной службы.
/// </summary>
/// <typeparam name="C">
///     Представление конфигурации службы.
/// </typeparam>
public partial class SC<C> : SC;
