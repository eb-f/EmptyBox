using EmptyBox.Construction.Machines;
using EmptyBox.Presentation.Containers;

using System.Diagnostics.CodeAnalysis;

namespace EmptyBox.Application.Services;

public sealed class QualifiedStateMachineFactory : IServiceFactory<IStateMachine>, ISingleton<QualifiedStateMachineFactory>
{
    public static QualifiedStateMachineFactory Instance { get; } = new();

    private QualifiedStateMachineFactory() { }

    [RequiresDynamicCode("Конструирование машины состояний.")]
    IStateMachine IServiceFactory<IStateMachine>.Initialize<[DynamicallyAccessedMembers(QualifiedStateMachine<S>.DynamicallyAccessedMembers)] S>(out S service)
    {
        return Initialize(out service);
    }

    [RequiresDynamicCode("Конструирование машины состояний.")]
    public S Initialize<[DynamicallyAccessedMembers(QualifiedStateMachine<S>.DynamicallyAccessedMembers)] S>()
        where S : class, IService<SI>
    {
        return (S)(object)new QualifiedStateMachine<S>();
    }

    [RequiresDynamicCode("Конструирование машины состояний.")]
    public QualifiedStateMachine<S> Initialize<[DynamicallyAccessedMembers(QualifiedStateMachine<S>.DynamicallyAccessedMembers)] S>(out S service)
        where S : class, IService<SI>
    {
        QualifiedStateMachine<S> machine = new();
        service = (S)(object)machine;

        return machine;
    }
}
