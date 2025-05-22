using EmptyBox.Application.Services.Operation;
using EmptyBox.Application.Services.Shared;
using EmptyBox.Construction.Machines;
using EmptyBox.Presentation.Permissions;

using System.Runtime.InteropServices;
using System.Threading;

namespace EmptyBox.Application.Services.Benchmarks.Teapots;

[DynamicInterfaceCastableImplementation]
public partial interface SyncTeapot<out SQ> : ITeapot, IConfigurableService<SQ, ITeapot.Configuration>
    where SQ : class, IState
{
    [Qualified<SC>]
    [return: SwitchTo<Launched>]
    protected void Launch()
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
    protected void Stop()
    {
        _ = Switch<SC<Configuration>>();
    }

    [Qualified<Launched>]
    [return: SwitchTo<Launched>]
    protected void Heat()
    {
        if (State is Launched launched)
        {
            if (double.IsFinite(launched.Configuration.HeatingRate)/* && launched.Configuration.HeatingRate > 0*/)
            {
                Heating heatingState = Switch<Heating>();

                int intervals = checked((int)((100 - double.Clamp(heatingState.Temperature, 0, 100)) / heatingState.Configuration.HeatingRate));

                for (int count = 0; count < intervals; count++)
                {
                    Thread.Sleep(100);
                    heatingState.Temperature += heatingState.Configuration.HeatingRate;
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
    protected void Maintenance()
    {
        var configured = SyncTeapotProxy.Stop((SyncTeapot<ISL>)this);
        SyncTeapotProxy.Launch(configured);
    }
}
