namespace EmptyBox.Generation.Abstractions.Resolution;

internal interface IAnalyzer<C, R>
{
    public R Analyze(C context);
}
