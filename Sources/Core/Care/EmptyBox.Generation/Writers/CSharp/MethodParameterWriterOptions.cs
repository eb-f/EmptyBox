namespace EmptyBox.Generation.Writers.CSharp;

internal readonly struct MethodParameterWriterOptions
{
    public TypePresentationOptions TypePresentation { get; init; }
    public WriterPresentationStyle Style { get; init; }
    public TypeTransformationOptions TypeTransformation { get; init; }
}
