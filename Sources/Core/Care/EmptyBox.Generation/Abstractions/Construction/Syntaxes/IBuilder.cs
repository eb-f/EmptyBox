using EmptyBox.Generation.Writers;

namespace EmptyBox.Generation.Abstractions.Construction.Syntaxes;

internal interface IBuilder<C, W>
    where W : AbstractWriter<W>
{
    public void Build(C context, W writer);
}