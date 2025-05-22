using EmptyBox.Generation.Abstractions.Construction.Syntaxes.CSharp;
using EmptyBox.Generation.Writers.CSharp;

using Microsoft.CodeAnalysis;

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace EmptyBox.Generation.Proxies.Construction.Syntaxes.CSharp.Internal.Qualificators;

internal sealed class MethodBodyQualificationProxyBuilder : MethodBodyProxyBuilder
{
    public static MethodBodyQualificationProxyBuilder Instance { get; } = new();

    private MethodBodyQualificationProxyBuilder() { }

    [SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "Требуются чёткие границы блоков")]
    public override void Build(Context context, CSharpWriter writer)
    {
        INamedTypeSymbol invalidQualificationException = context.Symbols.Qualification.Base.InvalidQualificationException;

        writer.Append("if ");

        using (var qualificationCheckScope = writer.AppendScope(Writers.ScopeBracketFraming.Round, Writers.ScopeParameters.NoIndent))
        {
            writer.Append(context.MethodWriterOptions.Presentation.InvocationTarget!)
                  .Append(".Qualification.IsAssignableTo(typeof(")
                  .AppendType(context.QualificationContext.Value.Qualification, options: context.OriginTypeWriterOptions)
                  .Append("))");
        }

        writer.AppendLine();

        using (var fulfilledContractScope = writer.AppendScope())
        {
            ImmutableQueue<MethodBodyProxyBuilder> sequencedBuilders = context.SequencedBuilders.Dequeue(out MethodBodyProxyBuilder next);
            next.Build(context with { SequencedBuilders = sequencedBuilders }, writer);
        }

        writer.AppendLine()
              .AppendLine("else");

        using (var contractViolationScope = writer.AppendScope())
        {
            OptimizedExceptionThrowBuilder.Context exceptionContext = new()
            {
                ReturnParameterUsage = context.MethodWriterOptions.Presentation.ReturnParameterUsage switch
                {
                    MethodReturnParameterUsage.Await => MethodReturnParameterUsage.Value,
                    MethodReturnParameterUsage value => value
                },
                Symbols = context.Symbols,
                Target = invalidQualificationException
            };

            if (context.Report.ServiceReport.HasValue)
            {
                exceptionContext = exceptionContext with
                {
                    ReturnType = context.TargetType,
                    TypePresentation = context.SwitchedTypeWriterOptions.Presentation,
                    TypeTransformation = context.SwitchedTypeWriterOptions.Transformation
                };
            }
            else if (context.Report.Target.Symbol.ReturnsVoid)
            {
                exceptionContext = exceptionContext with
                {
                    TypePresentation = context.OriginTypeWriterOptions.Presentation,
                    TypeTransformation = context.OriginTypeWriterOptions.Transformation
                };
            }
            else
            {
                exceptionContext = exceptionContext with
                {
                    ReturnType = context.Report.Target.Symbol.ReturnType,
                    TypePresentation = context.OriginTypeWriterOptions.Presentation,
                    TypeTransformation = context.OriginTypeWriterOptions.Transformation
                };
            }

            OptimizedExceptionThrowBuilder.Instance.Build(exceptionContext, writer);
        }
    }
}
