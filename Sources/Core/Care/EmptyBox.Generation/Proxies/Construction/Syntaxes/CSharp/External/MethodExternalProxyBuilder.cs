using EmptyBox.Generation.Writers;
using EmptyBox.Generation.Writers.CSharp;

using Microsoft.CodeAnalysis;

using System.Diagnostics.CodeAnalysis;

namespace EmptyBox.Generation.Proxies.Construction.Syntaxes.CSharp.External;

internal sealed class MethodExternalProxyBuilder : MethodProxyBuilder
{
    private const string QUALIFIED_PARAMETER_NAME = "qualified";

    public static MethodExternalProxyBuilder Instance { get; } = new();

    private MethodExternalProxyBuilder() { }

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

        writer.AppendDocumentationInheritance(targetMethod);
        writer.AppendLine("[global::System.Diagnostics.DebuggerHidden, global::System.Diagnostics.StackTraceHidden]");
        writer.AppendLine("[global::System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]");

        writer.AppendAccessibility(context.Accessibility)
              .Append($" static ");

        if (context.Report.Target.IsExactlyUnsafe)
        {
            writer.Append("unsafe ");
        }

        writer.AppendMethod(targetMethod, options: new MethodWriterOptions()
        {
            Style = WriterPresentationStyle.Declaration,
            Transformation = new MethodTransformationOptions()
            {
                ParameterSubstitution = new MethodParameterTargetPrependSubstitution()
                {
                    IsExtension = true,
                    Await = false,
                    RefKind = context.Report.RefKind,
                    TargetName = QUALIFIED_PARAMETER_NAME,
                    TargetType = context.TargetType
                }.Prepend,
                ReturnParameterTransformation = context.Report.ServiceReport.HasValue
                                              ? switchedTypeOptions.Transformation
                                              : null,
                TypeParameterSubstitution = (writer, _, options) => writer.AppendTypeArguments(context.Report.ExternalProxyMethodTypeParameters, options with { Style = WriterPresentationStyle.Installation })
            },
            TypePresentation = originTypeOptions.Presentation,
            TypeTransformation = originTypeOptions.Transformation
        });

        writer.AppendLine();

        using (var constraintScope = writer.AppendScope(ScopeBracketFraming.None, ScopeParameters.NoIndent | ScopeParameters.Deferred))
        {
            writer.AppendTypeParameterConstraints(context.Report.ExternalProxyMethodTypeParameters, options: originTypeOptions);
        }

        using (var bodyScope = writer.AppendScope(ScopeBracketFraming.None))
        {
            MethodBodyExternalProxyBuilder.Instance.Build(new MethodBodyProxyBuilder.Context()
            {
                Report = context.Report,
                TargetType = context.TargetType,
                SequencedBuilders = [],
                Symbols = context.Symbols,
                OriginTypeWriterOptions = originTypeOptions,
                SwitchedTypeWriterOptions = switchedTypeOptions,
                QualificationContext = context.QualificationContext,
                MethodWriterOptions = new MethodWriterOptions()
                {
                    Presentation = new MethodPresentationOptions()
                    {
                        IsInvocation = true,
                        ReturnParameterUsage = context.Report.ServiceReport?.IsAsync switch
                        {
                            null => (targetMethod.ReturnsByRef || targetMethod.ReturnsByRefReadonly) switch
                            {
                                true => MethodReturnParameterUsage.Reference,
                                false => MethodReturnParameterUsage.Value
                            },
                            _ => MethodReturnParameterUsage.Value
                        }
                    },
                    Transformation = new MethodTransformationOptions()
                    {
                        ParameterSubstitution = new MethodParameterTargetPrependSubstitution()
                        {
                            IsExtension = true,
                            Await = false,
                            RefKind = context.Report.RefKind,
                            TargetName = QUALIFIED_PARAMETER_NAME,
                            TargetType = context.TargetType
                        }.Prepend,
                        TypeParameterSubstitution = (writer, _, options) => writer.AppendTypeArguments(context.Report.InternalProxyMethodTypeParameters, options)
                    },
                    TypePresentation = originTypeOptions.Presentation,
                    TypeTransformation = originTypeOptions.Transformation,
                },
                ResultPrepend = "=> "
            }, writer);
        }

        writer.AppendLine();
    }
}
