using EmptyBox.Generation.Abstractions.Construction.Syntaxes;
using EmptyBox.Generation.Writers.CSharp;

namespace EmptyBox.Generation.Proxies.Construction.Syntaxes.CSharp;

internal abstract partial class TypeProxyBuilder : IBuilder<TypeProxyBuilder.Context, CSharpWriter>
{
    public abstract string Name { get; }

    public abstract void Build(Context context, CSharpWriter writer);
}
