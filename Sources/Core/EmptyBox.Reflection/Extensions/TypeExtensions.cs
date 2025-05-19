using EmptyBox.Enumeration;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace EmptyBox.Reflection.Extensions;

public static class TypeExtensions
{
    private sealed class TypeAssignableEqualityComparer : IEqualityComparer<Type>
    {
        public static TypeAssignableEqualityComparer Instance { get; } = new(false);
        public static TypeAssignableEqualityComparer InversedInstance { get; } = new(true);

        public bool IsInversed { get; }

        private TypeAssignableEqualityComparer(bool isInversed)
        {
            IsInversed = isInversed;
        }

        public bool Equals(Type? x, Type? y)
        {
            return object.Equals(x, y) || x != null && y != null && (IsInversed ? y.IsAssignableFrom(x) : x.IsAssignableFrom(y));
        }

        public int GetHashCode(Type obj)
        {
            return obj.GetHashCode();
        }
    }

    private static readonly MethodInfo UNMANAGED_CHECK_METHOD_TEMPLATE = typeof(RuntimeHelpers).GetMethod(nameof(RuntimeHelpers.IsReferenceOrContainsReferences))!;

    public static bool IsConstructedGenericTypeAssignableFrom(this Type to, Type from)
    {
        if (!to.IsConstructedGenericType)
        {
            throw new ArgumentOutOfRangeException(nameof(to), $"Type {to} is not constructed generic type.");
        }
        else if (!from.IsConstructedGenericType)
        {
            throw new ArgumentOutOfRangeException(nameof(from), $"Type {from} is not constructed generic type.");
        }

        if (to.GetGenericTypeDefinition() == from.GetGenericTypeDefinition())
        {
            Type[] toArguments = to.GetGenericArguments();
            Type[] fromArguments = from.GetGenericArguments();

            for (int i0 = 0; i0 < toArguments.Length; i0++)
            {
                if (toArguments[i0].IsAssignableFrom(fromArguments[i0]) && (!toArguments[i0].IsGenericParameter || !toArguments[i0].IsGenericParameterReplaceableBy(fromArguments[i0])))
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Определяет, возможно ли подставить вместо параметра типа аргумент <paramref name="argument"/>.
    /// </summary>
    /// <param name="parameter">
    ///     Заменяемый параметр типа.
    /// </param>
    /// <param name="argument">
    ///     Аргумент типа, проверяемый на соблюдение ограничений параметра типа.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Возникает, когда <paramref name="parameter"/> не является параметром типа.
    /// </exception>
    [RequiresDynamicCode("Проверка типа на наличие ссылочных полей.")]
    public static bool IsGenericParameterReplaceableBy([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] this Type parameter, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicConstructors)] Type argument)
    {
        if (parameter.IsGenericParameter)
        {
            bool baseTypeConstraint;
            bool implementedInterfacesConstraint;
            bool defaultConstructorConstraint;
            bool nonNullableTypeValueConstraint;
            bool referenceTypeConstraint;
            bool unmanagedConstraint;
            bool allowRefLike;

            // Кэш во избежание множества запросов внутри лямбда-функций
            Type[] argumentInterfaces = argument.GetInterfaces();

            if (argument.IsGenericParameter)
            {
                baseTypeConstraint = parameter.BaseType == null || parameter.BaseType.IsAssignableFrom(argument.BaseType);
                implementedInterfacesConstraint = parameter.GetInterfaces().All(x => argumentInterfaces.Contains(x, TypeAssignableEqualityComparer.InversedInstance));

                defaultConstructorConstraint = !parameter.GenericParameterAttributes.Has(GenericParameterAttributes.DefaultConstructorConstraint) || argument.GenericParameterAttributes.Has(GenericParameterAttributes.DefaultConstructorConstraint);
                nonNullableTypeValueConstraint = !parameter.GenericParameterAttributes.Has(GenericParameterAttributes.NotNullableValueTypeConstraint) || argument.GenericParameterAttributes.Has(GenericParameterAttributes.NotNullableValueTypeConstraint);
                referenceTypeConstraint = !parameter.GenericParameterAttributes.Has(GenericParameterAttributes.ReferenceTypeConstraint) || argument.GenericParameterAttributes.Has(GenericParameterAttributes.ReferenceTypeConstraint);
                unmanagedConstraint = parameter.GetCustomAttribute<IsUnmanagedAttribute>() == null || argument.GetCustomAttribute<IsUnmanagedAttribute>() != null;

                allowRefLike = parameter.GenericParameterAttributes.Has(GenericParameterAttributes.AllowByRefLike) || !argument.GenericParameterAttributes.Has(GenericParameterAttributes.AllowByRefLike);
            }
            else
            {
                baseTypeConstraint = parameter.BaseType == null || parameter.BaseType.IsAssignableFrom(argument);
                implementedInterfacesConstraint = parameter.GetInterfaces().All(x => argumentInterfaces.Contains(x, TypeAssignableEqualityComparer.InversedInstance));

                defaultConstructorConstraint = !parameter.GenericParameterAttributes.Has(GenericParameterAttributes.DefaultConstructorConstraint) || argument.GetConstructors().Any(x => !x.IsPrivate && x.GetParameters().Length == 0);
                nonNullableTypeValueConstraint = !parameter.GenericParameterAttributes.Has(GenericParameterAttributes.NotNullableValueTypeConstraint) || !argument.IsNullable();
                referenceTypeConstraint = !parameter.GenericParameterAttributes.Has(GenericParameterAttributes.ReferenceTypeConstraint) || argument.IsClass || argument.IsInterface;
                unmanagedConstraint = parameter.GetCustomAttribute<IsUnmanagedAttribute>() == null || argument.IsUnmanaged();

                allowRefLike = parameter.GenericParameterAttributes.Has(GenericParameterAttributes.AllowByRefLike) || !argument.IsByRefLike;
            }

            return baseTypeConstraint && implementedInterfacesConstraint
                && defaultConstructorConstraint && nonNullableTypeValueConstraint && referenceTypeConstraint && unmanagedConstraint
                && allowRefLike;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(parameter), $"Type {parameter} is not generic parameter.");
        }
    }

    /// <summary>
    ///     Производит проверку типа структуры на отсутствие управляемых ссылок.
    /// </summary>
    /// <param name="type">
    ///     Тип, представляющий структуру.
    /// </param>
    /// <returns>
    ///     
    /// </returns>
    [RequiresDynamicCode("Проверка типа на наличие ссылочных полей.")]
    public static bool IsUnmanaged(this Type type)
    {
        try
        {
            return !(bool)UNMANAGED_CHECK_METHOD_TEMPLATE.MakeGenericMethod(type).Invoke(null, null)!;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    /// <summary>
    ///     Определяет, содержит ли инсталляция типа в качестве аргумента типа значение <paramref name="argument"/>.
    /// </summary>
    /// <param name="type">
    ///     Инсталляция типа.
    /// </param>
    /// <param name="argument">
    ///     Предполагаемый аргумент типа.
    /// </param>
    public static bool IsContainsGenericArgument(this Type type, Type argument)
    {
        foreach (Type genericArgument in type.GetGenericArguments())
        {
            if (genericArgument == argument || genericArgument.IsGenericType && genericArgument.IsContainsGenericArgument(argument))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Находит инсталляцию интерфейса <paramref name="template"/> без учёта параметров типа.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="template"></param>
    /// <returns>
    ///     Инсталляция интерфейса <paramref name="template"/> в типе <paramref name="type"/> или <see langword="null"/>, если таковой не имеется.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     Возникает в случае, если <paramref name="template"/> не является интерфейсом.
    /// </exception>
    public static Type? GetEquallyGenericDefinitionInterface([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] this Type type, Type template)
    {
        if (!template.IsInterface)
        {
            throw new ArgumentException($"Type {template} is not interface.", nameof(template));
        }
        else
        {
            Type templateGenericDefinition = template.GetGenericTypeDefinition();

            return type.GetGenericTypeDefinition() == templateGenericDefinition
                 ? type
                 : type.GetInterfaces()
                       .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == templateGenericDefinition);
        }
    }

    public static Type? GetGenericArgumentAtSamePosition(this Type type, Type template, Type argument)
    {
        if (!type.IsGenericType)
        {
            throw new ArgumentOutOfRangeException(nameof(type), $"Type {type} is not generic type.");
        }
        else if (!template.IsGenericType)
        {
            throw new ArgumentOutOfRangeException(nameof(template), $"Type {template} is not generic type.");
        }
        else if (type.GetGenericTypeDefinition() != template.GetGenericTypeDefinition())
        {
            throw new ArgumentException($"", nameof(template));
        }
        else
        {
            Type[] typeArguments = type.GetGenericArguments();
            Type[] templateArguments = template.GetGenericArguments();

            for (int i0 = 0; i0 < typeArguments.Length; i0++)
            {
                if (templateArguments[i0] == argument)
                {
                    return typeArguments[i0];
                }
                else if (templateArguments[i0].IsGenericType && typeArguments[i0].IsGenericType)
                {
                    Type? searchType = templateArguments[i0].GetEquallyGenericDefinitionInterface(typeArguments[i0]);

                    if (searchType != null)
                    {
                        Type? result = typeArguments[i0].GetGenericArgumentAtSamePosition(searchType, argument);

                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
            }

            return null;
        }
    }

    /// <summary>
    ///     Находит и подменяет параметры типа в <paramref name="container"/> так, чтобы он наследовал или реализовывал <paramref name="filling"/>.
    /// </summary>
    /// <param name="container">
    ///     Тип, реализующий или наследующий <paramref name="filling"/> без учёта параметров типа.
    /// </param>
    /// <param name="filling">
    ///     Тип, на подобии которого будет конструироваться результирующий тип.
    /// </param>
    /// <returns>
    ///     Сконструированный по образцу <paramref name="filling"/> тип.
    /// </returns>
    [RequiresDynamicCode("Конструирование новой инсталляции типа.")]
    public static Type MakeConstructedGenericTypeLike([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] this Type container, Type filling)
    {
        if (!container.IsConstructedGenericType)
        {
            throw new ArgumentOutOfRangeException(nameof(container), $"Type {container} is not constructed generic type.");
        }
        else if (!filling.IsConstructedGenericType)
        {
            throw new ArgumentOutOfRangeException(nameof(filling), $"Type {filling} is not constructed generic interface.");
        }

        Type form = filling.GetGenericTypeDefinition();
        Type containerGenericDefinition = container.GetGenericTypeDefinition();

        if (containerGenericDefinition == form)
        {
            return filling;
        }

        Type[] containerFormImplementations = container.GetInterfaces()
                                                       .Where(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == form)
                                                       .ToArray();

        if (containerFormImplementations.Length == 0)
        {
            throw new ArgumentException($"Type {container} does not implement interface of type {form}.", nameof(filling));
        }

        Type[] containerArguments = container.GetGenericArguments();
        Type[] newContainerArguments = new Type[containerArguments.Length];
        Type[] containerGenericArguments = containerGenericDefinition.GetGenericArguments();
        Type[][] containerFormGenericArguments = containerGenericDefinition.GetInterfaces()
                                                                           .Where(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == form)
                                                                           .Select(x => x.GetGenericArguments())
                                                                           .ToArray();
        Type[][] containerFormArguments = containerFormImplementations.Select(x => x.GetGenericArguments())
                                                                      .ToArray();
        Type[] fillingArguments = filling.GetGenericArguments();
        Type[] formArguments = form.GetGenericArguments();

        Exception? lastException = null;

        //Ищем по всем реализациям интерфейса form у container
        for (int i0 = 0; i0 < containerFormArguments.Length; i0++)
        {
            Array.Copy(containerArguments, newContainerArguments, containerArguments.Length);

            //Смотрим по каждому аргументу типа Form
            for (int i1 = 0; i1 < formArguments.Length; i1++)
            {
                Type currentFillingArgument = fillingArguments[i1];
                Type currentFillingFormArgument = formArguments[i1];
                Type currentContainerFormArgument = containerFormArguments[i0][i1];
                Type currentContainerFormGenericArgument = containerFormGenericArguments[i0][i1];

                //Если аргумент типа данной реализации Form не совпадает с аргументом типа у реализации Filler
                if (currentContainerFormArgument != currentFillingArgument)
                {
                    //Проверяем, содержит ли в себе Container данный аргумент
                    bool isContainerGenericParameter = currentContainerFormGenericArgument.IsGenericParameter && currentContainerFormGenericArgument.DeclaringType == containerGenericDefinition;
                    //Проверяем, является ли данный аргумент внешним
                    bool isExternalGenericParameter = currentContainerFormArgument.IsGenericParameter && currentContainerFormArgument.DeclaringType != containerGenericDefinition;
                    //Проверяем доступность простого присваивания
                    bool isGenericParameterReplaceable = isExternalGenericParameter
                                                       ? currentContainerFormArgument.IsGenericParameterReplaceableBy(currentFillingArgument)
                                                       : isContainerGenericParameter && currentContainerFormGenericArgument.IsGenericParameterReplaceableBy(currentFillingArgument);

                    if (isGenericParameterReplaceable)
                    {
                        newContainerArguments[Array.IndexOf(containerGenericArguments, currentContainerFormGenericArgument)] = currentFillingArgument;
                    }
                    //Иначе проверяем наличие ковариантности у универсального аргумента
                    else if (currentFillingFormArgument.GenericParameterAttributes.Has(GenericParameterAttributes.Covariant))
                    {
                        //Проверяем недоступность простого присваивания текущего аргумента данной реализации Form к реализации Filling
                        if (!currentFillingArgument.IsAssignableFrom(currentContainerFormArgument))
                        {
                            //Получаем все аргументы типа Container, которые используются в данной реализации Form
                            Type[] includedContainerArguments = containerGenericArguments.Where(currentContainerFormGenericArgument.IsContainsGenericArgument).ToArray();

                            //Проверяем, что и в аргументе Filling, и в текущей реализации Form есть аргументы типа 
                            if (currentFillingArgument.IsGenericType && includedContainerArguments.Length > 0)
                            {
                                //Ищем аргументы для замены
                                foreach (Type included in includedContainerArguments)
                                {
                                    Type? replacingType = null;

                                    //Выясняем, является ли или реализует ли currentContainerFormGenericArgument интерфейс currentFillingArgument в общем виде
                                    Type? replacingTypeProvider = currentContainerFormGenericArgument.GetGenericTypeDefinition() == currentFillingArgument.GetGenericTypeDefinition()
                                                               && currentContainerFormGenericArgument.IsConstructedGenericTypeAssignableFrom(currentFillingArgument)
                                                                ? currentContainerFormGenericArgument
                                                                : currentContainerFormGenericArgument.GetInterfaces()
                                                                                                     .FirstOrDefault(x => x.IsGenericType
                                                                                                                       && x.GetGenericTypeDefinition() == currentFillingArgument.GetGenericTypeDefinition()
                                                                                                                       && x.IsConstructedGenericTypeAssignableFrom(currentFillingArgument));

                                    if (replacingTypeProvider != null)
                                    {
                                        //При совпадении получаем аргумент на нужной позиции
                                        replacingType = currentFillingArgument.GetGenericArgumentAtSamePosition(replacingTypeProvider, included);
                                    }

                                    if (replacingType == null)
                                    {
                                        goto NEXT_ITERATION;
                                    }
                                    else
                                    {
                                        newContainerArguments[Array.IndexOf(containerGenericArguments, included)] = replacingType;
                                    }
                                }
                            }
                            else
                            {
                                goto NEXT_ITERATION;
                            }
                        }
                    }
                    else if (currentFillingFormArgument.GenericParameterAttributes.Has(GenericParameterAttributes.Contravariant))
                    {
                        lastException = new NotImplementedException();

                        goto NEXT_ITERATION;
                    }
                    else
                    {
                        goto NEXT_ITERATION;
                    }
                }
            }

            return containerGenericDefinition.MakeGenericType(newContainerArguments);

        NEXT_ITERATION:
            continue;
        }

        lastException ??= new NotSupportedException();

        throw lastException;
    }

    /// <summary>
    ///     Определяет допустимость присваивания значения <see langword="null"/> переменной данного типа.
    /// </summary>
    /// <param name="type">
    ///     Проверяемый тип.
    /// </param>
    public static unsafe bool IsNullable(this Type type)
    {
        return type == null || type.IsClass || type.IsArray || type.IsInterface || type.IsPointer || Nullable.GetUnderlyingType(type) != null;
    }
}
