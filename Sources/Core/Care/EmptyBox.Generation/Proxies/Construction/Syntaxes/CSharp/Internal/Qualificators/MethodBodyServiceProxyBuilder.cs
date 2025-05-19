using EmptyBox.Generation.Abstractions.Construction.Syntaxes.CSharp;
using EmptyBox.Generation.Writers.CSharp;

using Microsoft.CodeAnalysis;

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace EmptyBox.Generation.Proxies.Construction.Syntaxes.CSharp.Internal.Qualificators;

internal sealed class MethodBodyServiceProxyBuilder : MethodBodyProxyBuilder
{
    private const string CONTRACT_VIOLATION_LABEL = "CONTRACT_VIOLATION";
    private const string INNER_EXCEPTION_VARIABLE = "innerException";

    public static MethodBodyServiceProxyBuilder Instance { get; } = new();

    private MethodBodyServiceProxyBuilder() { }

    [SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "Требуются чёткие границы блоков")]
    public override void Build(Context context, CSharpWriter writer)
    {
        INamedTypeSymbol exception = context.Symbols.Base.Exception;
        INamedTypeSymbol contractViolationException = context.Symbols.Qualification.Service.ContractViolationException;

        writer.AppendType(exception)
              .Append("? ")
              .Append(INNER_EXCEPTION_VARIABLE)
              .AppendLine(" = null;");

        writer.AppendLine();

        writer.AppendLine("try");

        using (var contractVerifierTryScope = writer.AppendScope())
        {
            ImmutableQueue<MethodBodyProxyBuilder> sequencedBuilders = context.SequencedBuilders.Dequeue(out MethodBodyProxyBuilder next);

            next.Build(context with { SequencedBuilders = sequencedBuilders }, writer);

            writer.AppendLine()
                  .AppendLine();

            writer.Append("if ");

            using (var qualificationCheckScope = writer.AppendScope(Writers.ScopeBracketFraming.Round, Writers.ScopeParameters.NoIndent))
            {
                writer.Append(context.MethodWriterOptions.Presentation.InvocationTarget!)
                      .Append(" is ")
                      .AppendType(context.TargetType, options: context.SwitchedTypeWriterOptions)
                      .Append(" switched && switched.Qualification.IsAssignableTo(typeof(")
                      .AppendType(context.Report.ServiceReport.Value.SwitchToQualification, options: context.OriginTypeWriterOptions)
                      .Append("))");
            }

            writer.AppendLine();

            using (var fulfilledContractScope = writer.AppendScope())
            {
                writer.Append("return switched;");
            }

            writer.AppendLine()
                  .AppendLine("else");

            using (var contractViolationScope = writer.AppendScope())
            {
                writer.Append("goto ")
                      .Append(CONTRACT_VIOLATION_LABEL)
                      .Append(';');
            }
        }
        
        writer.AppendLine()
              .Append("catch (")
              .AppendType(exception)
              .Append(" exception) when (!qualified.Qualification.IsAssignableTo(typeof(")
              .AppendType(context.Report.ServiceReport.Value.SwitchToQualification, options: context.OriginTypeWriterOptions)
              .AppendLine(")))");

        using (var contractViolationScope = writer.AppendScope())
        {
            writer.Append(INNER_EXCEPTION_VARIABLE)
                  .Append(" = exception;");
        }

        writer.AppendLine()
              .AppendLine();

        writer.Append(CONTRACT_VIOLATION_LABEL)
              .AppendLine(':');

        OptimizedExceptionThrowBuilder.Instance.Build(new OptimizedExceptionThrowBuilder.Context()
        {
            InnerExceptionWriter = static writer => writer.Append(INNER_EXCEPTION_VARIABLE),
            ReturnParameterUsage = context.MethodWriterOptions.Presentation.ReturnParameterUsage switch
            {
                MethodReturnParameterUsage.Await => MethodReturnParameterUsage.Value,
                MethodReturnParameterUsage value => value
            },
            ReturnType = context.TargetType,
            Symbols = context.Symbols,
            Target = contractViolationException,
            TypePresentation = context.SwitchedTypeWriterOptions.Presentation,
            TypeTransformation = context.SwitchedTypeWriterOptions.Transformation
        }, writer);
    }
}
