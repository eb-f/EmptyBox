using EmptyBox.Application.Services.Operation;
using EmptyBox.Application.Services.Shared;
using EmptyBox.Construction.Machines;
using EmptyBox.Presentation.Permissions;

using System.Runtime.InteropServices;
using System.Threading;

namespace EmptyBox.Application.Services.Benchmarks.Teapots;

[DynamicInterfaceCastableImplementation]
public partial interface SealedSyncTeapot<out SQ> : ITeapot, IConfigurableService<SQ, ITeapot.Configuration>
    where SQ : class, IState
{
    [Qualified<SC>]
    [return: SwitchTo<Launched>]
    protected sealed void Launch()
    {
        if (State is SC<Configuration> configured)
        {
            _ = Switch(new Launched()
            {
                Temperature = configured.Configuration.BaseTemperature ?? 26,
            });
        }
    }

    [Qualified<ISL>]
    [return: SwitchTo<SC>]
    protected sealed void Stop()
    {
        _ = Switch<SC<Configuration>>();
    }

    [Qualified<Launched>]
    [return: SwitchTo<Launched>]
    protected sealed void Heat()
    {
        if (State is Launched launched)
        {
            if (double.IsFinite(launched.Configuration.HeatingRate)/* && launched.Configuration.HeatingRate > 0*/)
            {
                Heating heatingState = Switch<Heating>();

                int intervals = checked((int)((100 - double.Max(launched.Temperature, 100)) / launched.Configuration.HeatingRate));

                for (int count = 0; count < intervals; count++)
                {
                    Thread.Sleep(100);
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
    protected sealed void Maintenance()
    {
        var configured = SealedSyncTeapotProxy.Stop((SealedSyncTeapot<ISL>)this);
        SealedSyncTeapotProxy.Launch(configured);
    }
}
