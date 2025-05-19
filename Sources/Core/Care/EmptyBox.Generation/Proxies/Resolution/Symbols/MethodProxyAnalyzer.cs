using EmptyBox.Generation.Abstractions.Resolution;
using EmptyBox.Generation.Diagnostics;
using EmptyBox.Generation.Proxies.Resolution.Symbols.Qualification;

using Microsoft.CodeAnalysis;

using System.Collections.Immutable;
using System.Linq;

namespace EmptyBox.Generation.Proxies.Resolution.Symbols;

internal partial class MethodProxyAnalyzer : IAnalyzer<MethodProxyAnalyzer.Context, MethodProxyAnalyzer.Report?>
{
    public static MethodProxyAnalyzer Instance { get; } = new();

    private static bool AssertIsStatic(Context context)
    {
        if (context.Target.Symbol.IsStatic)
        {
            context.DiagnosticMessages.Add(new SymbolDiagnosticRecord()
            {
                Arguments = [context.Symbols.Qualification.Base.QualifiedAttribute!.Name],
                Identifier = DiagnosticIdentifier.EBSG0004,
                SyntaxReferences = context.Target.Origin.DeclaringSyntaxReferences,
            });

            return false;
        }
        else
        {
            return true;
        }
    }

    private static bool AssertAccessibility(Context context)
    {
        switch (context.Target.Symbol.DeclaredAccessibility)
        {
            case Accessibility.Private or Accessibility.ProtectedAndInternal or Accessibility.Protected:
                return true;

            default:
                context.DiagnosticMessages.Add(new SymbolDiagnosticRecord()
                {
                    Identifier = DiagnosticIdentifier.EBSG0005,
                    SyntaxReferences = context.Target.Origin.DeclaringSyntaxReferences,
                });

                return false;
        }
    }

    private MethodProxyAnalyzer() { }

    public Report? Analyze(Context context)
    {
        MethodQualificationAnalyzer.Report? qualificationReport = MethodQualificationAnalyzer.Instance.Analyze(context);
        MethodServiceAnalyzer.Report? serviceReport = MethodServiceAnalyzer.Instance.Analyze(context);

        if ((qualificationReport?.IsProxyRequired ?? false) || serviceReport != null)
        {
            bool isCorrect = true;

            isCorrect &= AssertIsStatic(context);
            isCorrect &= AssertAccessibility(context);

            if (isCorrect)
            {
                ImmutableArray<ITypeParameterSymbol> actualMethodTypeParameters = context.Target
                                                                                         .Symbol
                                                                                         .TypeParameters
                                                                                         .RemoveAll(static parameter => parameter.ConstraintTypes.Any(static c => c.IsSealed));

                return new Report()
                {
                    ExternalProxyMethodTypeParameters = [.. context.TypeReport.QualificationReport?.ParametersExceptQualifier ?? [], .. actualMethodTypeParameters],
                    InternalProxyMethodTypeParameters = [.. context.TypeReport.QualificationReport?.LikewiseQualifiedParameters ?? [], .. actualMethodTypeParameters],
                    QualificationReport = qualificationReport,
                    ServiceReport = serviceReport,
                    RefKind = context.TypeReport.Target.Symbol.TypeKind switch
                    {
                        TypeKind.Structure => context.Target.Symbol.IsReadOnly switch
                        {
                            false => RefKind.Ref,
                            true => RefKind.In
                        },
                        _ => RefKind.None
                    },
                    Target = context.Target,
                };
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
}
