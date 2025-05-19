namespace EmptyBox.Generation.Writers;

internal enum ScopeBracketFraming
{
    None,
    /// <summary>
    ///     '<![CDATA[<]]>' и '<![CDATA[>]]>'
    /// </summary>
    Angle,
    /// <summary>
    ///     '{' и '}'
    /// </summary>
    Curly,
    /// <summary>
    ///     '[' и ']'
    /// </summary>
    Square,
    /// <summary>
    ///     '(' и ')'
    /// </summary>
    Round,
}
