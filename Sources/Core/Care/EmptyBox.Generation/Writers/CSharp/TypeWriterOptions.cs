namespace EmptyBox.Generation.Writers.CSharp;

internal readonly struct TypeWriterOptions
{
    public TypePresentationOptions Presentation { get; init; }
    public WriterPresentationStyle Style { get; init; }
    public TypeTransformationOptions Transformation { get; init; }
}
