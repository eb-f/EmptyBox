using EmptyBox.Generation.Writers.CSharp;

namespace EmptyBox.Generation.Proxies.Construction.Syntaxes.CSharp.Internal;

internal sealed class MethodBodyInternalProxyBuilder : MethodBodyProxyBuilder
{
    public static MethodBodyInternalProxyBuilder Instance { get; } = new MethodBodyInternalProxyBuilder();

    private MethodBodyInternalProxyBuilder() { }

    public override void Build(Context constructionContext, CSharpWriter writer)
    {
        writer.Append(constructionContext.ResultPrepend)
              .AppendMethod(constructionContext.Report.Target.Symbol, options: constructionContext.MethodWriterOptions)
              .Append(';');
    }
}
