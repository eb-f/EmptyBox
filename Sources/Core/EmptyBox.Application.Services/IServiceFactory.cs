using EmptyBox.Construction.Machines;

using System.Diagnostics.CodeAnalysis;

namespace EmptyBox.Application.Services;

/// <summary>
///     Контракт фабрики служб.
/// </summary>
public interface IServiceFactory<out M>
    where M : class, IStateMachine
{
    /// <summary>
    ///     Создаёт машину состояний и инициализирует в ней службу <typeparamref name="S"/>.
    /// </summary>
    /// <typeparam name="S">
    ///     Контракт службы.
    /// </typeparam>
    /// <returns>
    ///     Экземпляр службы в состоянии <see cref="SI"/>.
    /// </returns>
    [RequiresDynamicCode("Конструирование машины состояний.")]
    public S Initialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] S>()
        where S : class, IService<SI>;

    /// <summary>
    ///     Создаёт машину состояний и инициализирует в ней службу <typeparamref name="S"/>.
    /// </summary>
    /// <typeparam name="S">
    ///     Контракт службы.
    /// </typeparam>
    /// <param name="service">
    ///     Экземпляр службы в состоянии <see cref="SI"/>.
    /// </param>
    /// <returns>
    ///     Экземпляр машины состояний.
    /// </returns>
    [RequiresDynamicCode("Конструирование машины состояний.")]
    public M Initialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] S>(out S service)
        where S : class, IService<SI>;
}
