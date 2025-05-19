namespace EmptyBox.Generation.Writers.CSharp;

internal readonly struct MethodPresentationOptions
{
    public string? InvocationTarget { get; init; }
    public bool IsStatic { get; init; }
    public bool IsInvocation { get; init; }
    public bool ExcludeTypeArguments { get; init; }
    public MethodReturnParameterUsage ReturnParameterUsage { get; init; }
}
