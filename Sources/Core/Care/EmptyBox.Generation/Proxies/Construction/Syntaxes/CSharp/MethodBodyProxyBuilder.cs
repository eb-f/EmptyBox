using EmptyBox.Generation.Abstractions.Construction.Syntaxes;
using EmptyBox.Generation.Writers.CSharp;

namespace EmptyBox.Generation.Proxies.Construction.Syntaxes.CSharp;

internal abstract partial class MethodBodyProxyBuilder : IBuilder<MethodBodyProxyBuilder.Context, CSharpWriter>
{
    public abstract void Build(Context context, CSharpWriter writer);
}
