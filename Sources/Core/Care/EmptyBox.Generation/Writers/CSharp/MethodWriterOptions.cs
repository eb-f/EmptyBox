namespace EmptyBox.Generation.Writers.CSharp;

internal readonly struct MethodWriterOptions
{
    public TypePresentationOptions TypePresentation { get; init; }
    public TypeTransformationOptions TypeTransformation { get; init; }
    public WriterPresentationStyle Style { get; init; }
    public MethodTransformationOptions Transformation { get; init; }
    public MethodPresentationOptions Presentation { get; init; }
}
