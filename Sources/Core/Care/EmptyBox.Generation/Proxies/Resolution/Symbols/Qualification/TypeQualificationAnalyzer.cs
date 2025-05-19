using EmptyBox.Generation.Abstractions.Resolution;
using EmptyBox.Generation.Diagnostics;
using EmptyBox.Generation.Extensions;

using Microsoft.CodeAnalysis;

using System.Collections.Immutable;
using System.Linq;

namespace EmptyBox.Generation.Proxies.Resolution.Symbols.Qualification;

internal partial class TypeQualificationAnalyzer : IAnalyzer<TypeProxyAnalyzer.Context, TypeQualificationAnalyzer.Report?>
{
    public static TypeQualificationAnalyzer Instance { get; } = new();

    private TypeQualificationAnalyzer() { }

    public Report? Analyze(TypeProxyAnalyzer.Context context)
    {
        if (context.Symbols.Qualification.Base.IsAvailable)
        {
            #region Выпрямление доступа
            INamedTypeSymbol targetType = context.Target.Symbol;
            INamedTypeSymbol qualifiedInterface = context.Symbols.Qualification.Base.IQualified_1!;
            #endregion

            INamedTypeSymbol[] implementedQualifiedInterfaces = targetType.AllInterfaces
                                                                          .Where(@interface => @interface.IsGenericType
                                                                                            && SymbolEqualityComparer.Default.Equals(@interface.ConstructUnboundGenericType(), qualifiedInterface))
                                                                          .ToArray();

            if (implementedQualifiedInterfaces.Length == 1)
            {
                INamedTypeSymbol implementedQualifiedInterface = implementedQualifiedInterfaces[0];

                if (implementedQualifiedInterface.TypeArguments[0] is ITypeParameterSymbol qualifierParameter)
                {
                    ImmutableArray<ITypeParameterSymbol> parametersExceptQualifier = context.Target.Symbol.TypeParameters.Remove(qualifierParameter, SymbolEqualityComparer.Default);

                    return new Report()
                    {
                        ParametersExceptQualifier = parametersExceptQualifier,
                        LikewiseQualifiedParameters = parametersExceptQualifier.RemoveAll(parameter => !parameter.ContainsTypeArgument(qualifierParameter)),
                        QualifierParameter = qualifierParameter
                    };
                }
                else
                {
                    context.DiagnosticMessages.Add(new SymbolDiagnosticRecord()
                    {
                        Arguments = [targetType.Name, implementedQualifiedInterface.Name, implementedQualifiedInterface.TypeArguments[0]],
                        Identifier = DiagnosticIdentifier.EBSG0002,
                        SyntaxReferences = context.Target.Symbol.DeclaringSyntaxReferences,
                    });
                }
            }
            else if (implementedQualifiedInterfaces.Length > 1)
            {
                context.DiagnosticMessages.Add(new SymbolDiagnosticRecord()
                {
                    Arguments = [targetType.Name, qualifiedInterface.Name],
                    Identifier = DiagnosticIdentifier.EBSG0001,
                    SyntaxReferences = context.Target.Symbol.DeclaringSyntaxReferences,
                });
            }
        }

        return null;
    }
}
