using EmptyBox.Generation.Writers.CSharp;

namespace EmptyBox.Generation.Proxies.Construction.Syntaxes.CSharp.External;

internal sealed class MethodBodyExternalProxyBuilder : MethodBodyProxyBuilder
{
    public static MethodBodyExternalProxyBuilder Instance { get; } = new();

    private MethodBodyExternalProxyBuilder() { }

    public override void Build(Context context, CSharpWriter writer)
    {
        writer.Append(context.ResultPrepend)
              .AppendType(context.TargetType, options: context.OriginTypeWriterOptions)
              .Append('.')
              .Append(context.TargetType.Name)
              .Append("Proxy.")
              .AppendMethod(context.Report.Target.Origin, options: context.MethodWriterOptions)
              .Append(';');
    }
}
