using EmptyBox.Application.Services.Operation;
using EmptyBox.Construction.Machines;
using EmptyBox.Presentation.Permissions;

using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace EmptyBox.Application.Services.Shared;

/// <summary>
///     Служба выставочного автоматизированного самовара.
/// </summary>
/// <typeparam name="SQ">
///     Представление состояния службы.
/// </typeparam>
/// <remarks>
///     Чаю?
/// </remarks>
[DynamicInterfaceCastableImplementation]
public partial interface Teapot<out SQ> : ITeapot, IManageableService<SQ, ITeapot.Launched>, IConfigurableService<SQ, ITeapot.Configuration>
    where SQ : class, IState
{
    // Реализация метода расширения контракта машины состояний
    ValueTask IManageableService<SQ>.Launch(CancellationToken cancellationToken)
    {
        // Свойство State представлено типом IState, поэтому требуется преобразование
        // Однако, машина состояний гарантирует, что свойство State содержит значения,
        // представимые типами, перечисленными в атрибутах Qualified
        if (State is SC<Configuration> configured)
        {
            // Переключаемся в состояние "Запущено"
            // Значение свойства Configuration состояния SC<Configuration> переносится в состояние Launched автоматически
            // машиной состояний при помощи вызова метода Map
            _ = Switch(new Launched()
            {
                Temperature = configured.Configuration.BaseTemperature ?? 26,
            });
        }

        return ValueTask.CompletedTask;
    }

    ValueTask IManageableService<SQ>.Stop(CancellationToken cancellationToken)
    {
        _ = Switch<SC<Configuration>>();

        return ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Нагревает содержимое.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <remarks>
    ///     Тратит некоторое время на поднятие температуры.
    /// </remarks>
    [Qualified<Launched>]
    [return: SwitchTo<Launched>]
    protected async ValueTask Heat(CancellationToken cancellationToken = default)
    {
        // Свойство State представлено типом IState, поэтому требуется преобразование
        // Однако, машина состояний гарантирует, что свойство State содержит значения,
        // представимые типами, перечисленными в атрибутах Qualified
        if (State is Launched launched)
        {
            // Закомментированная, или отсутствующая, проверка на неравенство 0 значения HeatingRate
            // является бессознательным нарушением контракта, ведь в методе присутствует деление на данное значение
            // и последующее преобразование результата к типу Int32
            if (double.IsFinite(launched.Configuration.HeatingRate)/* && launched.Configuration.HeatingRate > 0*/)
            {
                // Переключаемся в состояние "Нагревание"
                // Значение свойств Configuration и Temperature состояния Launched переносится в состояние Heating автоматически
                // машиной состояний при помощи вызова метода Map
                // Не является нарушением контракта, если после этого вызова будет выполнен вызов Switch<Launched>()
                Heating heatingState = Switch<Heating>();

                // При делении на 0 и последующем преобразовании double.PositiveInfinity к типу Int32
                // будет создано исключение System.OverflowException, что прервёт исполнение метода
                // и приведёт к нарушению контракта
                int intervals = checked((int)((100 - double.Clamp(heatingState.Temperature, 0, 100)) / heatingState.Configuration.HeatingRate));

                for (int count = 0; count < intervals; count++)
                {
                    await Task.Delay(100, cancellationToken);
                    heatingState.Temperature += heatingState.Configuration.HeatingRate;
                }

                // Переключаемся в состояние "Запущено"
                // Значение свойств Configuration и Temperature состояния Heating переносится в состояние Launched автоматически
                // машиной состояний при помощи вызова метода Map
                _ = Switch<Launched>();
            }
            else
            {
                // Сознательное нарушение - состояние по завершению метода Heat не представимо указанным типом в атрибуте SwitchTo
                // По завершению исполнения метода Heat будет сгенерировано исключение ContractViolationException
                // Отловив данное исключение можно проверить состояния службы и произвести необходимые действия
                // Например, вызвать метод Maintenance
                _ = Switch<RequireMaintenance>();
            }
        }
    }

    /// <summary>
    ///     Обслуживание самовара.
    /// </summary>
    /// <param name="cancellationToken">
    ///     Токен отмены действия.
    /// </param>
    /// <remarks>
    ///     А вы пробовали перезагрузить устройство?
    /// </remarks>
    [Qualified<RequireMaintenance>]
    [return: SwitchTo<Launched>]
    protected async ValueTask Maintenance(CancellationToken cancellationToken = default)
    {
        // В отличии от метода Switch контракта IStateMachineContract
        // методы, определённые в интерфейсах, наследующихся от IService<SQ> зависят от состояния.
        // Поэтому, после смены состояния методом Switch, вызов таких методов будет являться ошибкой -
        // машина состояний создаст исключение ContractViolationException.
        // Для правильной типизации используем внутреннюю прослойку из интерфейса TeapotProxy
        var configured = await TeapotProxy.Stop((Teapot<ISL>)this, cancellationToken);
        await TeapotProxy.Launch(configured, cancellationToken);
    }
}
