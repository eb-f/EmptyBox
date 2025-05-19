namespace EmptyBox.Presentation.Containers;

/// <summary>
///     Интерфейс типа, подразумевающего наличие лишь единственного экземпляра данного типа.
/// </summary>
/// <typeparam name="S">
///     Представление экземпляра.
/// </typeparam>
public interface ISingleton<out S>
    where S : class?, ISingleton<S>?
{
    /// <summary>
    ///     Единственный экземпляр типа.
    /// </summary>
    public static abstract S Instance { get; }
}
