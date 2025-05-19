using EmptyBox.Construction.Machines;
using EmptyBox.Execution;
using EmptyBox.Presentation.Containers;
using EmptyBox.Presentation.Permissions;
using EmptyBox.Reflection.Extensions;

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace EmptyBox.Application.Services;

/// <summary>
///     Машина состояний.
/// </summary>
/// <typeparam name="S">
///     Контракт службы.
/// </typeparam>
[RequiresDynamicCode("Конструирование машины состояний.")]
public class QualifiedStateMachine<[DynamicallyAccessedMembers(DynamicallyAccessedMembers)] S> : IStateMachine, IDynamicInterfaceCastable
    where S : class, IService<SI>
{
    /// <summary>
    ///     Перечисление членов контракта, доступ к которым осуществляется при помощи рефлексии.
    /// </summary>
    /// <remarks>
    ///     Вспомогательное значение для AOT-компиляции.
    /// </remarks>
    public const DynamicallyAccessedMemberTypes DynamicallyAccessedMembers = DynamicallyAccessedMemberTypes.Interfaces;

    /// <summary>
    ///     Слабая ссылка на кэш вариаций контракта <typeparamref name="S"/> для различных состояний.
    /// </summary>
    private static readonly WeakRef<ConcurrentDictionary<Type, Type>> ContractVariationStorageWeakReference = new();

    private static readonly Type RepresentationBase;

    /// <summary>
    ///     Ленивый конструктор кэша вариаций контракта.
    /// </summary>
    private static ConcurrentDictionary<Type, Type> ContractVariationStorage => LazyInitializer.EnsureInitialized(ref ContractVariationStorageWeakReference.Value, static () =>
    {
        ConcurrentDictionary<Type, Type> storage = new();
        storage[typeof(SI)] = typeof(S);

        return storage;
    });

    /// <summary>
    ///     Определяет базовую корректность контракта <typeparamref name="S"/>.
    /// </summary>
    protected static bool IsContractValid { get; }

    static QualifiedStateMachine()
    {
        IsContractValid = typeof(S).IsInterface
                       && typeof(S).IsGenericType
                       && typeof(S).GetCustomAttribute<DynamicInterfaceCastableImplementationAttribute>() != null;

        RepresentationBase = IsContractValid
                           ? AdoptContract(typeof(IState), typeof(S))
                           : typeof(void);
    }

    /// <summary>
    ///     Адаптирует тип контракта к иному состоянию.
    /// </summary>
    /// <param name="stateType">
    ///     Тип состояния, в котором будет находится адаптированный контракт.
    /// </param>
    /// <param name="contractType">
    ///     Тип контракта, для которого будет проводится адаптация.
    /// </param>
    /// <returns>
    ///     Адаптированный контракт.
    /// </returns>
    /// <remarks>
    ///     Сигнатура метода соответствует параметру <see langword="valueFactory"/> метода <see cref="ConcurrentDictionary{TKey, TValue}.GetOrAdd{TArg}(TKey, Func{TKey, TArg, TValue}, TArg)"/>.
    /// </remarks>
    
    private static Type AdoptContract(Type stateType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type contractType)
    {
        return contractType.MakeConstructedGenericTypeLike(typeof(IQualified<>).MakeGenericType(stateType));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static R DynamicInterfaceCastThrow<R>([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type interfaceType)
    {
        if (interfaceType.IsAssignableTo(RepresentationBase))
        {
            return IException.Throw<InvalidQualificationException, R>();
        }
        else if (interfaceType.IsGenericType && interfaceType.IsAssignableTo(typeof(IQualified<IState>)))
        {
            try
            {
                Type upcastedInterface = AdoptContract(typeof(IState), interfaceType);

                if (upcastedInterface.IsAssignableFrom(RepresentationBase))
                {
                    goto INVALID_QUALIFICATION_EXCEPTION;
                }
            }
            catch { }

        }

        return IException.Throw<InvalidOperationException, R>();

    INVALID_QUALIFICATION_EXCEPTION:
        return IException.Throw<InvalidQualificationException, R>();
    }

    /// <summary>
    ///     Кэш вариаций контракта <typeparamref name="S"/> для различных состояний.
    /// </summary>
    private readonly ConcurrentDictionary<Type, Type> ContractVariations = ContractVariationStorage;

    /// <remarks>
    ///     Требуется для инициализации без выполнения логики в методе <see langword="set"/> свойства <see cref="State"/>.
    /// </remarks>
    private IState _State = SI.Instance;

    public event StateSwitchedEventHandler? StateSwitched;

    [SuppressMessage("Trimming", "IL2111:Method with parameters or return value with `DynamicallyAccessedMembersAttribute` is accessed via reflection. Trimmer can't guarantee availability of the requirements of the method.", Justification = "Соответствующий параметр типа отмечен атрибутом.")]
    public IState State
    {
        get => _State;
        protected set
        {
            if (value.GetType() != _State.GetType())
            {
                Contract = ContractVariations.GetOrAdd(value.GetType(), AdoptContract, Contract);
            }

            _State = value;
        }
    }
    public Type Contract { get; private set; } = typeof(S);

    public QualifiedStateMachine()
    {
        if (!IsContractValid)
        {
            throw new InvalidContractException();
        }
    }

    SQ IStateMachineContract.Switch<SQ>(SQ newState)
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

    RuntimeTypeHandle IDynamicInterfaceCastable.GetInterfaceImplementation(RuntimeTypeHandle interfaceTypeHandle)
    {
        Type interfaceType = Type.GetTypeFromHandle(interfaceTypeHandle)!;

        if (interfaceType == Contract || Contract.IsAssignableTo(interfaceType))
        {
            return Contract.TypeHandle;
        }

        return DynamicInterfaceCastThrow<RuntimeTypeHandle>(interfaceType);
    }

    bool IDynamicInterfaceCastable.IsInterfaceImplemented(RuntimeTypeHandle interfaceTypeHandle, bool throwIfNotImplemented)
    {
        Type interfaceType = Type.GetTypeFromHandle(interfaceTypeHandle)!;

        if (interfaceType == Contract || Contract.IsAssignableTo(interfaceType))
        {
            return true;
        }
        else if (throwIfNotImplemented)
        {
            return DynamicInterfaceCastThrow<bool>(interfaceType);
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    ///     Вызывает событие <see cref="StateSwitched"/>.
    /// </summary>
    /// <param name="previousState">
    ///     Предшествующее состояние.
    /// </param>
    protected virtual void OnStateSwitch(IState previousState)
    {
        if (!ReferenceEquals(previousState, State))
        {
            StateSwitched?.Invoke(this, previousState, State);
        }
    }
}
