using System;

namespace EmptyBox.Construction.Machines;

/// <summary>
///     Атрибут, позволяющий сгенерировать класс состояния на основе интерфейса, к которому данный атрибут применён.
/// </summary>
[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
public class StateAttribute : Attribute;
