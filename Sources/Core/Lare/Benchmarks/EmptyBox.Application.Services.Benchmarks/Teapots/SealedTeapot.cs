using EmptyBox.Application.Services.Operation;
using EmptyBox.Application.Services.Shared;
using EmptyBox.Construction.Machines;
using EmptyBox.Presentation.Permissions;

using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace EmptyBox.Application.Services.Benchmarks.Teapots;

[DynamicInterfaceCastableImplementation]
public partial interface SealedTeapot<out SQ> : ITeapot, IManageableService<SQ, ITeapot.Launched>, IConfigurableService<SQ, ITeapot.Configuration>
    where SQ : class, IState
{
    ValueTask IManageableService<SQ>.Launch(CancellationToken cancellationToken)
    {
        if (State is SC<Configuration> configured)
        {
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

    [Qualified<Launched>]
    [return: SwitchTo<Launched>]
    protected sealed async ValueTask Heat(CancellationToken cancellationToken = default)
    {
        if (State is Launched launched)
        {
            if (double.IsFinite(launched.Configuration.HeatingRate)/* && launched.Configuration.HeatingRate > 0*/)
            {
                Heating heatingState = Switch<Heating>();

                int intervals = checked((int)((100 - double.Max(launched.Temperature, 100)) / launched.Configuration.HeatingRate));

                for (int count = 0; count < intervals; count++)
                {
                    await Task.Delay(100, cancellationToken);
                    heatingState.Temperature += launched.Configuration.HeatingRate;
                }

                _ = Switch<Launched>();
            }
            else
            {
                _ = Switch<RequireMaintenance>();
            }
        }
    }

    [Qualified<RequireMaintenance>]
    [return: SwitchTo<Launched>]
    protected sealed async ValueTask Maintenance(CancellationToken cancellationToken = default)
    {
        var configured = await SealedTeapotProxy.Stop((SealedTeapot<ISL>)this, cancellationToken);
        await SealedTeapotProxy.Launch(configured, cancellationToken);
    }
}
