using EmptyBox.Generation.Abstractions.Construction.Syntaxes;
using EmptyBox.Generation.Writers.CSharp;

namespace EmptyBox.Generation.Proxies.Construction.Syntaxes.CSharp;

internal abstract partial class MethodProxyBuilder : IBuilder<MethodProxyBuilder.Context, CSharpWriter>
{
    public abstract void Build(Context methodContext, CSharpWriter writer);
}