using System;

namespace EmptyBox.Generation.Polyfills;

/// <summary>
///     Определяет состав специализации атрибута.
/// </summary>
[Flags]
public enum AttributeSpecializationComposition
{
    /// <summary>
    ///     Включает все варианты состава специализации атрибута.
    /// </summary>
    All = 0b00,
    /// <summary>
    ///     Исключает из состава атрибуты без специализации.
    /// </summary>
    ExcludeOverall = 0b01,
    /// <summary>
    ///     Исключает из состава атрибуты со смешанной специализацией.
    /// </summary>
    ExcludeComposite = 0b10,
    /// <summary>
    ///     Исключает из состава все специализации, кроме явно указанной.
    /// </summary>
    OnlyTarget = ExcludeOverall | ExcludeComposite,
}
