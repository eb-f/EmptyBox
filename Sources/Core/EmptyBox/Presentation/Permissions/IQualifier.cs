namespace EmptyBox.Presentation.Permissions;

/// <summary>
///     Контракт квалификатора, или иначе - специализации.
/// </summary>
public interface IQualifier
{
    /// <summary>
    ///     Проверяет наличие квалификатора <typeparamref name="QFlag"/> в определении квалификатора <typeparamref name="QSet"/>.
    /// </summary>
    /// <typeparam name="QSet">
    ///     Набор квалификаторов.
    /// </typeparam>
    /// <typeparam name="QFlag">
    ///     Квалификатор, присутствие которого необходимо проверить.
    /// </typeparam>
    public static bool Has<QSet, QFlag>()
        where QSet : IQualifier
        where QFlag : IQualifier
    {
        return typeof(QSet).IsAssignableTo(typeof(QFlag));
    }

    /// <summary>
    ///     Проверяет на равенство квалификаторы <typeparamref name="QLeft"/> и <typeparamref name="QRight"/>
    /// </summary>
    /// <typeparam name="QLeft">
    ///     Левый квалификатор.
    /// </typeparam>
    /// <typeparam name="QRight">
    ///     Правый квалификатор.
    /// </typeparam>
    public static bool Equals<QLeft, QRight>()
        where QLeft : IQualifier
        where QRight : IQualifier
    {
        return typeof(QLeft) == typeof(QRight);
    }
}
