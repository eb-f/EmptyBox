using EmptyBox.Presentation.Containers;

namespace EmptyBox.Construction.Machines;

/// <summary>
///     Представляет изначальное состояние службы.
/// </summary>
public sealed class SI : IState, ISingleton<SI>
{
    public static SI Instance { get; } = new();

    private SI() { }

    public void Map<N>(N state)
        where N : class, IState
    {

    }
}