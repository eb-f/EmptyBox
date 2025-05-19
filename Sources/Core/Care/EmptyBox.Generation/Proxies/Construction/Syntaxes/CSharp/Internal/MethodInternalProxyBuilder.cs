using EmptyBox.Generation.Proxies.Construction.Syntaxes.CSharp.Internal;
using EmptyBox.Generation.Proxies.Construction.Syntaxes.CSharp.Internal.Qualificators;
using EmptyBox.Generation.Writers.CSharp;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EmptyBox.Generation.Proxies.Construction.Syntaxes.CSharp;

internal sealed partial class MethodInternalProxyBuilder : MethodProxyBuilder
{
    private const string QUALIFIED_PARAMETER_NAME = "qualified";

    public static MethodInternalProxyBuilder Instance { get; } = new();

    private MethodInternalProxyBuilder() { }

    [SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "Требуются чёткие границы блоков")]
    public override void Build(Context context, CSharpWriter writer)
    {
        #region Выпрямление доступа
        IMethodSymbol targetMethod = context.Report.Target.Symbol;
        ITypeParameterSymbol? qualifiedTypeParameter = context.QualificationReport?.QualifierParameter;
        #endregion

        TypeWriterOptions originTypeOptions = new()
        {
            Transformation = new TypeTransformationOptions()
            {
                NameSubstitution = new ProxyTypeParameterNameModifier(context.TargetType, context.Report.InternalProxyMethodTypeParameters).Format,
                TypeSubstitution = new TypeSubstitution(qualifiedTypeParameter, context.QualificationContext?.Qualification).Replace
            }
        };
        TypeWriterOptions switchedTypeOptions = originTypeOptions with
        {
            Transformation = originTypeOptions.Transformation with
            {
                TypeSubstitution = new TypeSubstitution((context.Symbols.Base.Void, context.Report.ServiceReport?.OverriddenReturnType),
                                                        (context.Symbols.Base.ValueTask[0], context.Report.ServiceReport?.OverriddenReturnType),
                                                        (context.Symbols.Base.Task[0], context.Report.ServiceReport?.OverriddenReturnType),
                                                        (qualifiedTypeParameter, context.Report.ServiceReport?.SwitchToQualification)).Replace
            }
        };

        writer.AppendLine("/// <summary>");
        writer.Append("///     Автоматически сгенерированный метод-прослойка для метода <see cref=\"")
              .AppendMethod(targetMethod, options: new MethodWriterOptions() { Style = WriterPresentationStyle.Documentation })
              .AppendLine("\"/>.");
        writer.AppendLine("/// </summary>");

        writer.AppendLine("[global::System.Diagnostics.StackTraceHidden]");

        if (context.Symbols.Compilation.Options.OptimizationLevel == OptimizationLevel.Debug)
        {
            //writer.AppendLine("[global::System.Diagnostics.DebuggerHidden]");
            writer.AppendLine("[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]");
        }

        writer.AppendAccessibility(context.Accessibility)
              .Append($" static ");

        if (context.Report.Target.IsExactlyUnsafe)
        {
            writer.Append("unsafe ");
        }

        if (context.Report.ServiceReport?.IsAsync ?? false)
        {
            writer.Append("async ");
        }

        writer.AppendMethod(targetMethod, options: new MethodWriterOptions()
        {
            Style = WriterPresentationStyle.Declaration,
            Transformation = new MethodTransformationOptions()
            {
                ParameterSubstitution = new MethodParameterTargetPrependSubstitution()
                {
                    IsExtension = false,
                    Await = false,
                    RefKind = context.Report.RefKind,
                    TargetName = QUALIFIED_PARAMETER_NAME,
                    TargetType = context.TargetType
                }.Prepend,
                ReturnParameterTransformation = context.Report.ServiceReport.HasValue
                                              ? switchedTypeOptions.Transformation
                                              : null,
                TypeParameterSubstitution = (writer, _, options) => writer.AppendTypeArguments(context.Report.InternalProxyMethodTypeParameters, options)
            },
            TypePresentation = originTypeOptions.Presentation,
            TypeTransformation = originTypeOptions.Transformation
        });

        writer.AppendLine();

        using (var bodyScope = writer.AppendScope())
        {
            Stack<MethodBodyProxyBuilder> bodyBuilders = [];
            bodyBuilders.Push(MethodBodyInternalProxyBuilder.Instance);

            if (context.Report.ServiceReport != null)
            {
                bodyBuilders.Push(MethodBodyServiceProxyBuilder.Instance);
            }

            if (context.QualificationContext != null)
            {
                bodyBuilders.Push(MethodBodyQualificationProxyBuilder.Instance);
            }

            bodyBuilders.Pop().Build(new MethodBodyProxyBuilder.Context()
            {
                Report = context.Report,
                TargetType = context.TargetType,
                SequencedBuilders = [.. bodyBuilders],
                Symbols = context.Symbols,
                OriginTypeWriterOptions = originTypeOptions,
                SwitchedTypeWriterOptions = switchedTypeOptions,
                QualificationContext = context.QualificationContext,
                MethodWriterOptions = new MethodWriterOptions()
                {
                    Presentation = new MethodPresentationOptions()
                    {
                        InvocationTarget = QUALIFIED_PARAMETER_NAME,
                        IsInvocation = true,
                        ReturnParameterUsage = context.Report.ServiceReport?.IsAsync switch
                        {
                            true => MethodReturnParameterUsage.Await,
                            false => MethodReturnParameterUsage.Value,
                            null => (targetMethod.ReturnsByRef || targetMethod.ReturnsByRefReadonly) switch
                            {
                                true => MethodReturnParameterUsage.Reference,
                                false => MethodReturnParameterUsage.Value
                            }
                        }
                    },
                    TypePresentation = originTypeOptions.Presentation,
                    TypeTransformation = originTypeOptions.Transformation,
                },
                ResultPrepend = context.Report.ServiceReport.HasValue || targetMethod.ReturnsVoid
                              ? ""
                              : "return "
            }, writer);
        }
    }
}
