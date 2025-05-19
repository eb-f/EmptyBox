namespace EmptyBox.Application.Services.Operation;

/// <summary>
///     Контракт состояния запущенной службы.
/// </summary>
public interface ISL : ISC;

/// <summary>
///     Контракт состояния запущенной службы.
/// </summary>
/// <typeparam name="C">
///     Представление конфигурации.
/// </typeparam>
public interface ISL<C> : ISL, ISC<C>;