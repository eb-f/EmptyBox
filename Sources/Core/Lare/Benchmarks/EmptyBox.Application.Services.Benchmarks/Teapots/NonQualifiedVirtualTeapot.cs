using EmptyBox.Application.Services.Operation;
using EmptyBox.Application.Services.Shared;
using EmptyBox.Construction.Machines;
using EmptyBox.Execution;
using EmptyBox.Presentation.Permissions;

using System;
using System.Threading;
using System.Threading.Tasks;

using static EmptyBox.Application.Services.Shared.ITeapot;

namespace EmptyBox.Application.Services.Benchmarks.Teapots;

internal class NonQualifiedVirtualTeapot(Configuration configuration) : ITeapot, IStateMachine
{
    public event StateSwitchedEventHandler? StateSwitched;

    public IState State { get; private set; } = new SC<Configuration>() { Configuration = configuration };
    public Type Contract => typeof(NonQualifiedTeapot);

    protected void OnStateSwitch(IState previousState)
    {
        if (!Equals(previousState, State))
        {
            StateSwitched?.Invoke(this, previousState, State);
        }
    }

    protected SQ Switch<SQ>()
        where SQ : class, IState, new()
    {
        return Switch(new SQ());
    }

    protected SQ Switch<SQ>(SQ newState)
        where SQ : class, IState
    {
        IState oldState = State;
        oldState.Map(newState);
        State = newState;
        OnStateSwitch(oldState);

        if (oldState is IDisposable disposableState)
        {
            disposableState.Dispose();
        }

        return newState;
    }

    public virtual ValueTask Launch(CancellationToken cancellationToken = default)
    {
        if (State is SC<Configuration> configured)
        {
            Exception? innerException = null;

            try
            {
                _ = Switch(new Launched()
                {
                    Temperature = configured.Configuration.BaseTemperature ?? 26,
                });

                if (State is Launched)
                {
                    return ValueTask.CompletedTask;
                }
                else
                {
                    goto CONTRACT_VIOLATION;
                }
            }
            catch (Exception exception) when (State is not Launched)
            {
                innerException = exception;
            }

        CONTRACT_VIOLATION:
            return IException.Throw<ContractViolationException, ValueTask>(innerException: innerException);
        }
        else
        {
            return IException.Throw<InvalidQualificationException, ValueTask>();
        }
    }

    public virtual ValueTask Stop(CancellationToken cancellationToken = default)
    {
        if (State is ISL)
        {
            Exception? innerException = null;

            try
            {
                _ = Switch<SC<Configuration>>();

                if (State is SC<Configuration>)
                {
                    return ValueTask.CompletedTask;
                }
                else
                {
                    goto CONTRACT_VIOLATION;
                }
            }
            catch (Exception exception) when (State is not SC<Configuration>)
            {
                innerException = exception;
            }

        CONTRACT_VIOLATION:
            return IException.Throw<ContractViolationException, ValueTask>(innerException: innerException);
        }
        else
        {
            return IException.Throw<InvalidQualificationException, ValueTask>();
        }
    }

    public virtual async ValueTask Heat(CancellationToken cancellationToken = default)
    {
        if (State is Launched launched)
        {
            Exception? innerException = null;

            try
            {
                if (double.IsFinite(launched.Configuration.HeatingRate)/* && launched.Configuration.HeatingRate > 0*/)
                {
                    Heating heatingState = Switch<Heating>();

                    int intervals = checked((int)((100 - double.Clamp(heatingState.Temperature, 0, 100)) / heatingState.Configuration.HeatingRate));

                    for (int count = 0; count < intervals; count++)
                    {
                        await Task.Delay(100, cancellationToken);
                        heatingState.Temperature += heatingState.Configuration.HeatingRate;
                    }

                    _ = Switch<Launched>();
                }
                else
                {
                    _ = Switch<RequireMaintenance>();
                }

                if (State is Launched)
                {
                    return;
                }
                else
                {
                    goto CONTRACT_VIOLATION;
                }
            }
            catch (Exception exception) when (State is not Launched)
            {
                innerException = exception;
            }

        CONTRACT_VIOLATION:
            IException.Throw<ContractViolationException>(innerException: innerException);
        }
        else
        {
            IException.Throw<InvalidQualificationException>();
        }
    }
}
